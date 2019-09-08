namespace Konpaku
{
    internal class Progress
    {
        internal static States CurrentState;

        internal enum States
        {
            BeginInitializingPackage,
            EndInitializingPackage,
            BeginSelectingMod,
            EndSelectingMod
        }
    }
}