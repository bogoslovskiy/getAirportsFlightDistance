namespace getAirportsFlightDistance.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Services.Models;

    // Решение для демонстрации общего подхода к кэшированию.
    // - Сервис описаний аэропортов может быть внешним сервисом, который не управляется "нами",
    //   соответственно не стоит полагаться на стабильность и производительность его ответов, поэтому их стоит кэшировать,
    //   тем более данные имеют справочный характер.
    // - В рабочем окружении REST сервисы могут масштабироваться, кроме того, могут быть еще сервисы и воркеры
    //   в приватной сети, которые потенциально могли бы использовать кэшированные данные.
    //   Поэтому в рабочей системе для этого лучше воспользоваться отдельным сервисом/компонентой кэширования,
    //   за которым/ой скрыто какое-нибудь быстрое хранилище, например, Redis и ему подобные.
    public class CachedAirportDescriptionService : ICachedAirportDescriptionService
    {
        private readonly ConcurrentDictionary<string, AirportDescription> iataCodeToAirportDescriptionMap =
            new(comparer: StringComparer.OrdinalIgnoreCase);
        
        private readonly IAirportDescriptionService airportDescriptionService;

        public CachedAirportDescriptionService(IAirportDescriptionService airportDescriptionService)
        {
            this.airportDescriptionService = airportDescriptionService;
        }

        public async Task<AirportDescription> GetDescription(string iata)
        {
            if (iataCodeToAirportDescriptionMap.TryGetValue(iata, out AirportDescription airportDescription))
            {
                return airportDescription;
            }

            // Конечно могут быть конкурентные запросы за одним и тем же объектом.
            // Если по каким-то причинам хочется делать только 1 запрос в сервис, и отдавать всем конкурентным
            // запросам тот же ответ, то конкурентные запросы надо ставить в блокировку.
            // Такое решение реализуемо, например, с помощью обертки с async (чтобы не забирать поток у системы) семафором + SpinLock/SpinWait.
            // Но это более сложное решение, которое не всегда стоит затрат на него.
            // В этом коде продемонстрировано простое решение, последний конкурентный поток перезапишет уже существующий объект,
            // думаю ничего страшного.
            // Если говорить о стабильном отказоустойчивом решении, то нужно применить предложение выше + request policy с несколькими попытками запроса
            // в нужный сервис при неудаче, или вообще пойти по пути реализации Job'а / Worker'а, который в фоне с некоторой периодичностью инвалидирует кэш,
            // и является единственным источником записи, т.е. избавиться от конкурентности, а потоки чтения заставлять ожидать, но в неблокирующем стиле с применением async семафора.
            
            airportDescription = await airportDescriptionService.GetDescription(iata);
            
            iataCodeToAirportDescriptionMap.TryAdd(iata, airportDescription);

            return airportDescription;
        }
    }
}