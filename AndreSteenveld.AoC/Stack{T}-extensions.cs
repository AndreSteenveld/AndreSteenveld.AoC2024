namespace AndreSteenveld.AoC;

public static class Stack_T__extensions
{
    public static void Replace<T>(this Stack<T> @this, T element) {
        @this.Pop();
        @this.Push(element);
    }
}