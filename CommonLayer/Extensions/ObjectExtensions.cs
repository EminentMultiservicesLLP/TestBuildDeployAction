using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLayer.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }

        public static bool IsNotNull<T>(this T obj)
        {
            return obj != null;
        }

        public static T IfNull<T>(this T obj, Action action) where T:class
        {
            if (obj == null)
                action();

            return obj;
        }

        public static T IfNull<T>(this T obj, Func<T> action) where T : class
        {
            if (obj == null)
                action();

            return obj;
        }

        public static R IfNotNull<T, R>(this T obj, Func<T, R> action) where T : class
        {
            if (obj != null && action != null)
                return action(obj);

            return default(R);
        }

        public static T IfNotNull<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null && action != null)
            {
                action(obj);
                if (obj is Exception)
                    throw new Exception((obj as Exception).Message);
            }

            return obj;
        }

        public static R IfNull<T, R>(this T obj, Func<T, R> action) where T : class
        {
            if (obj == null && action != null)
                return action(obj);

            return default(R);
        }

        public static T Finally<T>(this T obj, Action action) where T : class
        {
            action();
            return obj;
        }

        public static T Finally<T>(this T obj, Action<T> action) where T : class
        {
            action(obj);
            return obj;
        }

    }

}
