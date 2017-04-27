using System;

namespace Fences
{
    public static class PositionCalculator
    {

        public static int[] Divide(int lenght, int index, int n)
        {
            if (lenght < 200)
                throw new ArgumentException("Такой длины не бывает: " + lenght);
            int firstLen = 185;
            int lastLen = 185;

            if (index == 0)
                firstLen = 100;
            if (index == n - 1)
                lastLen = 100;

            if (lenght < firstLen + 190 + lastLen)
            {
                if (index == 0)
                    return new[] {firstLen, lenght - firstLen};
                return new[] {lenght - lastLen, lastLen};
            }

            int middleLen = lenght - firstLen - lastLen;
            int numSeg = middleLen % 1200 == 0 ? middleLen / 1200 : middleLen / 1200 + 1;
            int minSegLenght = middleLen / numSeg / 10 * 10;
            int rest = middleLen - numSeg * minSegLenght;
            int[] result = new int[numSeg + 2];
            result[0] = firstLen;
            result[result.Length - 1] = lastLen;

            for (int i = 1; i < result.Length - 1; i++)
            {
                result[i] = minSegLenght;
                int curRest = Math.Min(rest, 10);
                result[i] += curRest;
                rest -= curRest;
            }

            return result;
        }
    }
}