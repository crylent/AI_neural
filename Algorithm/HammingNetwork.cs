using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AI_neural.Image;

namespace AI_neural.Algorithm;

public class HammingNetwork
{
    private readonly float[][] _weights;
    
    private const int Size = BitImage8.Size * BitImage8.Size;
    private readonly float _shift;
    private readonly float _epsilon;
    private const float K1 = 1f / Size;
    private const float Un = 1f / K1;
    
    public HammingNetwork(IReadOnlyList<BitImage8> samples)
    {
        _weights = new float[Size][];
        for (var i = 0; i < Size; i++)
        {
            _weights[i] = new float[samples.Count];
        }
        for (var x = 0; x < samples.Count; x++)
        {
            var sample = samples[x];
            var y = 0;
            foreach (var point in sample.GetIterator())
            {
                _weights[y][x] = point / 2f;
                y++;
            }
        }

        _shift = Size / 2f;
        _epsilon = 1f / samples.Count;
    }

    public int FindBestMatch(BitImage8 img)
    {
        var u = new float[_weights[0].Length];
        for (var i = 0; i < _weights[0].Length; i++)
        {
            var z = _shift;
            for (var j = 0; j < Size; j++)
            {
                z += _weights[j][i] * img.GetPoint(j);
            }
            
            u[i] = z switch
            {
                < 0 => 0,
                > Un => Un * K1,
                _ => z * K1
            };
        }
        Debug.Print($"{string.Join(" ", u)}");

        int bestMatch;
        while (MaxnetStep(ref u, out bestMatch)) {}

        return bestMatch;
    }

    private bool MaxnetStep(ref float[] u, out int lastNotZero)
    {
        var sum = u.Sum();
        var zeros = 0;
        lastNotZero = 0;
        for (var i = 0; i < u.Length; i++)
        {
            var a = u[i] - _epsilon * (sum - u[i]);
            if (a > 0)
            {
                u[i] = a;
                lastNotZero = i;
            }
            else
            {
                u[i] = 0;
                zeros++;
            }
        }
        Debug.Print($"{string.Join(" ", u)}");
        return zeros < u.Length - 1;
    }
}