using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AI_neural.Algorithm;

public class HebbNetwork: NeutralNetwork
{
    private readonly List<int[]> _weights = new();
    

    public override int FindBestMatch(BitImage8 img)
    {
        var response = GetResponse(img);
        var one = NoMatch;
        for (var i = 0; i < response.Length; i++)
        {
            if (response[i] != 1) continue;
            if (one == NoMatch) one = i;
            else return NoMatch;
        }
        return one;
    }

    private int[] GetResponse(BitImage8 img)
    {
        var response = new int[_weights.Count];
        for (var iSample = 0; iSample < _weights.Count; iSample++)
        {
            response[iSample] = _weights[iSample][0];
            for (var iBit = 1; iBit <= ImageSize; iBit++)
            {
                response[iSample] += img.GetPoint(iBit - 1) * _weights[iSample][iBit];
            }
        }
        Debug.Print($"Response: {string.Join(" ", response)}");
        return response.Select(r => r > 0 ? 1 : -1).ToArray();
    }

    public void Learn(BitImage8[] samples)
    {
        var responses = new int[samples.Length][];
        for (var i = 0; i < samples.Length; i++)
        {
            responses[i] = Enumerable.Repeat(-1, samples.Length + _weights.Count).ToArray();
            responses[i][i + _weights.Count] = 1;
        }
        Learn(samples, responses);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void Learn(BitImage8[] samples, int[][] responses)
    {
        for (var i = 0; i < samples.Length; i++)
        {
            _weights.Add(Zeros());
        }

        var canFinishLearning = false;
        while (!canFinishLearning)
        {
            canFinishLearning = true;
            for (var i = 0; i < samples.Length; i++)
            {
                if (ResponsesAreEqual(GetResponse(samples[i]), responses[i])) continue;
                canFinishLearning = false;
                LearnSampleStep(samples[i], responses[i]);
            }
        }

        for (var i = 0; i < _weights.Count; i++)
        {
            Debug.Print($"Weights[{i}]: {string.Join(" ", _weights[i])}");
        }
    }

    private void LearnSampleStep(BitImage8 sample, IReadOnlyList<int> response)
    {
        for (var iSample = 0; iSample < _weights.Count; iSample++)
        {
            _weights[iSample][0] += response[iSample];
            for (var iBit = 1; iBit <= ImageSize; iBit++)
            {
                _weights[iSample][iBit] += sample.GetPoint(iBit - 1) * response[iSample];
            }
        }
    }

    private static bool ResponsesAreEqual(IReadOnlyList<int> a, IReadOnlyList<int> b)
    {
        if (a.Count != b.Count) return false;
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private static int[] Zeros() => Enumerable.Repeat(0, ImageSize + 1).ToArray();
}