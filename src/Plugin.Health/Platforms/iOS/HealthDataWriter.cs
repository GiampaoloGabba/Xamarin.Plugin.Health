using System;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using static Plugin.Health.HealthKitData;

namespace Plugin.Health
{
    public class HealthDataWriter : HealthDataWriterBase
    {
        readonly HealthService _healthService;
        readonly HKHealthStore _healthStore;

        public HealthDataWriter(HealthService healthService)
        {
            _healthService = healthService;
            _healthStore = healthService.HealthStore;
        }

        public override async Task<bool> WriteAsync(HealthDataType healthDataType, double value, DateTime start, DateTime? end = null)
        {
            if (end == null)
            {
                end = start;
            }

            var healthKit = healthDataType.ToHealthKit();

            if (healthKit.HKType == HKTypes.Category)
            {
                var type = HKCategoryType.Create(healthDataType.ToHealthKit().CategoryTypeIdentifier);
                var sample = HKCategorySample.FromType(type, (nint)value, (NSDate)end, (NSDate)start);

                var (success, error) = await _healthStore.SaveObjectAsync(sample).ConfigureAwait(false);

                return success;

            }
            else if (healthKit.HKType == HKTypes.Quantity)
            {

                var type = HKQuantityType.Create(healthDataType.ToHealthKit().QuantityTypeIdentifier);
                var quantity = HKQuantity.FromQuantity(healthDataType.ToHealthKit().Unit, value);
                var sample = HKQuantitySample.FromType(type, quantity, (NSDate)end, (NSDate)start);

                var (success, error) = await _healthStore.SaveObjectAsync(sample).ConfigureAwait(false);

                return success;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public async Task<bool> SaveBmi(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.iOS_BodyMassIndex, value, start, end);
        }

        public async Task<bool> SaveMindfulSession(int value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.MindfulSession, value, start, end);
        }

        public async Task<bool> SaveHeight(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.Height, value, start, end);
        }

        public async Task<bool> SaveWeight(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.Weight, value, start, end);
        }

        public async Task<bool> SaveStep(int value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.StepCount, value, start, end);
        }
    }
}