using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI_neural;

[DataContract]
public class BitImage8
{
    public const int Size = 8;
    [DataMember] private readonly byte[] _rows = new byte[Size];

    public void DrawPoint(int x, int y)
    {
        _rows[y] |= (byte) (1 << x);
    }
    
    public void ClearPoint(int x, int y)
    {
        _rows[y] &= (byte) ~(1 << x);
    }

    public int GetPoint(int x, int y) => (_rows[y] & (1 << x)) > 0 ? 1 : - 1;

    public int GetPoint(int n) => GetPoint(n % Size, n / Size);

    public IEnumerable<int> GetIterator()
    {
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                yield return GetPoint(x, y);
            }
        }
    }

    public BitImage8 Clone()
    {
        var clone = new BitImage8();
        for (var i = 0; i < _rows.Length; i++)
        {
            clone._rows[i] = _rows[i];
        }
        return clone;
    }
}