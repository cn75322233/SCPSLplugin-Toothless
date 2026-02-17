using System;

namespace SCP181
{
    public static class RandomUtils
    {
        // 每次调用都创建新的 Random，基于当前时间
        public static int Range(int min, int max)
        {
            return new Random((int)DateTime.Now.Ticks).Next(min, max);
        }

        // 百分比判断（1-100）
        public static bool RollPercent(int chance)
        {
            return new Random((int)DateTime.Now.Ticks).Next(1, 101) <= chance;
        }
    }
}