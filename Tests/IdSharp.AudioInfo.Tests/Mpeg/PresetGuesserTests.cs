using System.Collections.Generic;
using System.Linq;

using IdSharp.AudioInfo.Inspection;

using NUnit.Framework;

namespace IdSharp.Mpeg;

[TestFixture]
public class PresetGuesserTests
{
    [Test]
    public void PresetGuesser_ShouldReturnExpectedTable()
    {
        var expected = new List<(byte t1, byte t2, byte t3, byte t4, byte t5, byte t6, byte t7, LamePreset preset, LameVersionGroup vg1, LameVersionGroup? vg2, LameVersionGroup? vg3)>
    {
        (255, 58, 1, 1, 3, 2, 205, LamePreset.Insane, LameVersionGroup.lvg390_3901_392, null, null),
        (255, 58, 1, 1, 3, 2, 206, LamePreset.Insane, LameVersionGroup.lvg3902_391, LameVersionGroup.lvg3931_3903up, null),
        (255, 57, 1, 1, 3, 4, 205, LamePreset.Insane, LameVersionGroup.lvg394up, null, null),
        // ⚠️ Truncated: Add the rest of the rows here...
    };

        //var actual = PresetGuesser();
        //List<PresetGuesser> actual = null; // Replace with actual call to PresetGuesser() method

        Assert.That(actual.Count, Is.EqualTo(expected.Count), "PresetGuesser entry count mismatch");

        for (var i = 0; i < expected.Count; i++)
        {
            var act = actual[i];
            var (tv1, tv2, tv3, tv4, tv5, tv6, tv7, preset, vg1, vg2, vg3) = expected[i];

            var msg = $"Row {i}: ";

            Assert.That(act.TVs[0], Is.EqualTo(tv1), msg + "TV1 mismatch");
            Assert.That(act.TVs[1], Is.EqualTo(tv2), msg + "TV2 mismatch");
            Assert.That(act.TVs[2], Is.EqualTo(tv3), msg + "TV3 mismatch");
            Assert.That(act.TVs[3], Is.EqualTo(tv4), msg + "TV4 mismatch");
            Assert.That(act.TVs[4], Is.EqualTo(tv5), msg + "TV5 mismatch");
            Assert.That(act.TVs[5], Is.EqualTo(tv6), msg + "TV6 mismatch");
            Assert.That(act.TVs[6], Is.EqualTo(tv7), msg + "TV7 mismatch");

            Assert.That(act.Res, Is.EqualTo(preset), msg + "Preset mismatch");

            Assert.That(act.VGs[0], Is.EqualTo(vg1), msg + "VG1 mismatch");
            Assert.That(act.VGs[1], Is.EqualTo(vg2 ?? LameVersionGroup.None), msg + "VG2 mismatch");
            Assert.That(act.VGs[2], Is.EqualTo(vg3 ?? LameVersionGroup.None), msg + "VG3 mismatch");
        }
    }

    [Test]
    public void PresetGuesser_OutputIsStable()
    {
        IList<PresetGuessRow> actual = PresetGuesser();

        var expected = new List<PresetGuessRow>
        {
            new PresetGuessRow(
                vg1: LameVersionGroup.lvg390_3901_392,
                tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 205,
                result: LamePreset.Insane),

            new PresetGuessRow(
                vg1: LameVersionGroup.lvg3902_391,
                vg2: LameVersionGroup.lvg3931_3903up,
                tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 206,
                result: LamePreset.Insane),

            new PresetGuessRow(
                vg1: LameVersionGroup.lvg394up,
                tv1: 255, tv2: 57, tv3: 1, tv4: 1, tv5: 3, tv6: 4, tv7: 205,
                result: LamePreset.Insane),

            // ... Add the rest of the entries here using named parameters ...

            new PresetGuessRow(
                vg1: LameVersionGroup.lvg394up,
                tv1: 16, tv2: 57, tv3: 2, tv4: 1, tv5: 0, tv6: 4, tv7: 56,
                result: LamePreset.Phone)
        };

        Assert.That(actual.Count, Is.EqualTo(expected.Count), "Preset row count mismatch");

        for (int i = 0; i < expected.Count; i++)
        {
            var a = actual[i];
            var e = expected[i];

            Assert.That(a.Res, Is.EqualTo(e.Res), $"Row {i} - Res mismatch");

            for (int t = 0; t < 7; t++)
                Assert.That(a.TVs[t], Is.EqualTo(e.TVs[t]), $"Row {i} - TVs[{t}] mismatch");

            for (int v = 0; v < 3; v++)
                Assert.That(a.VGs[v], Is.EqualTo(e.VGs[v]), $"Row {i} - VGs[{v}] mismatch");
        }
    }
}
