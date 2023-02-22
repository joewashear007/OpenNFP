namespace OpenNFP.Client.Utils
{
    public class EnumSliderHelper<T> where T : struct, Enum
    {
        public EnumSliderHelper(Action<T> setProp)
        {
            this.setProp = setProp;
        }
        public EnumSliderHelper()
        {
            this.setProp = null;
        }

        private T _value = default;
        private readonly Action<T>? setProp;

        public string[] Labels => Enum.GetNames<T>();
        public int Min => Enum.GetValues<T>().Cast<int>().Min();
        public int Max => Enum.GetValues<T>().Cast<int>().Max();
        public int ValueInt
        {
            get => (int)(object)_value;
            set
            {
                T newValue = (T)(object)value;
                _value = newValue;
                setProp?.Invoke(newValue);
            }
        }
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                setProp?.Invoke(value);
            }
        }
    }
}
