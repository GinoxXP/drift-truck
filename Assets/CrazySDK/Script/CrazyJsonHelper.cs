using System;
using UnityEngine;

namespace CrazyGames
{
    public static class CrazyJsonHelper
    {
        public static string ToJson<T>(T[] array)
        {
            var wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return FixJson(JsonUtility.ToJson(wrapper));
        }

        private static string FixJson(string value)
        {
            return value.Substring(9, value.Length - 10);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}