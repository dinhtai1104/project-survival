namespace Engine
{
    public class SharedValueBase<T> : IShareValue<T>
    {
        private T value;
        private string key;
        public T Value { get => value; set => this.value = value; }

        public string Key => key;
    }
}