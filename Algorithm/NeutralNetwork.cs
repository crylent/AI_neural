namespace AI_neural.Algorithm;

public abstract class NeutralNetwork
{
    protected const int ImageSize = BitImage8.Size * BitImage8.Size;
    public const int NoMatch = -1;

    public abstract int FindBestMatch(BitImage8 img);
}