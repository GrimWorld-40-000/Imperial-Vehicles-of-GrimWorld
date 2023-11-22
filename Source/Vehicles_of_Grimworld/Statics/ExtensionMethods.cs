namespace Vehicles_of_Grimworld
{
    public static class ExtensionMethods
    {
        public static float Map(this float value, float minSource, float maxSource, float minTarget, float maxTarget)
        {
            return (value - minSource) / (maxSource - minSource) * (maxTarget - minTarget) + minTarget;
        }
    }
}
