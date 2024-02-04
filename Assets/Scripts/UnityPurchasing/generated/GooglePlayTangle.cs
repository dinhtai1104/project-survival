// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("dcAGK2j8TSLmw+Ys/9MOpinwCDzQLDnRN7rewPG6wgqXAAl9XpiYVY7tervGbDrW3WhQn9uI0V2HP+CuMS0wHPSU3kkYLd9a2K2RxihqneVpl3P6orvBRdJ7/w/zbImT6/GQvkXIw2EVQBbhLoA93Q+ABvgCVc9CZSBH9HEzdGkIN5DPqTe1yZssPw54+/X6ynj78Ph4+/v6UMEOS50LFIcbxTZljJvPqWzD/TaivNIF/KYaDjOcbzfUSTLhlS6kCZ++6OTU3DXKePvYyvf889B8snwN9/v7+//6+cIza58BLLMHZJQGnYj+tTHzQb7ay6Di4LziZ/q1tOI9TfJxqOwIcJvJ7NyWy7WSiGJSz8t79ZqtNGRwJP+Hl2FUUOM6r/j5+/r7");
        private static int[] order = new int[] { 4,9,7,8,7,6,12,9,13,11,11,11,13,13,14 };
        private static int key = 250;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
