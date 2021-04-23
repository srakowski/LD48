namespace LD48
{
    struct Optional<T>
    {
        public Optional(T value)
        {
            HasValue = true;
            Value = value;
        }

        public bool HasValue { get; }

        public T Value { get; }
    }

    struct Either<TLeft, TRight>
    {
        public Either(TLeft left)
        {
            IsLeft = true;
            Left = left;
            IsRight = false;
            Right = default;
        }

        public bool IsLeft { get; }

        public TLeft Left { get; }

        public Either(TRight right)
        {
            IsLeft = false;
            Left = default;
            IsRight = true;
            Right = right;
        }

        public bool IsRight { get; }

        public TRight Right { get; }
    }
}
