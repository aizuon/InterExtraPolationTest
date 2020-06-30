namespace Client.Extensions
{
    public static class PositionUtils
    {
        public static int Lerp(int pos1, int pos2, float dt)
        {
            return (int)(pos1 * (1 - dt) + pos2 * dt);
        }
    }
}
