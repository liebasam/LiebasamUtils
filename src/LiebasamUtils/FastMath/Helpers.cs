namespace LiebasamUtils
{
    public static partial class FastMath
    {
        /// <summary>
        /// Tests if the type supports addition.
        /// </summary>
        static bool CanAdd<T>()
        {
            try
            {
                var x = default(T) + (dynamic)default(T);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests if the type supports multiplication.
        /// </summary>
        static bool CanMultiply<T>()
        {
            try
            {
                var x = default(T) * (dynamic)default(T);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
