using System;

namespace Dfe.Spi.GraphQlApi.Application.Loaders
{
    public class LearningProviderPointer
    {
        public string SourceSystemName { get; set; }
        public string SourceSystemId { get; set; }
        public DateTime? PointInTime { get; set; }
        public string[] Fields { get; set; }

        public override string ToString()
        {
            return $"learning-provider:{SourceSystemName.ToLower()}:{SourceSystemId.ToLower()}:{PointInTime:0}";
        }

        public override bool Equals(object obj)
        {
            return obj is LearningProviderPointer pointer &&
                   pointer.SourceSystemName.Equals(SourceSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                   pointer.SourceSystemId.Equals(SourceSystemId, StringComparison.InvariantCultureIgnoreCase) &&
                   pointer.PointInTime == PointInTime;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}