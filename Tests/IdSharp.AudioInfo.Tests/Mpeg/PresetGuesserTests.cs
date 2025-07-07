using System.Collections.Generic;
using System.Linq;

using IdSharp.AudioInfo.Inspection;

using NUnit.Framework;

namespace IdSharp.Mpeg;

[TestFixture]
public class PresetGuesserTests
{
    [Test]
    public void PresetGuesser_OutputIsStable()
    {
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
                    result: LamePreset.Phone),

            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 1, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.FastExtreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.FastExtreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, vg3: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.FastStandard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.Medium),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.FastMedium),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 195, result: LamePreset.R3mix),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 196, result: LamePreset.R3mix),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 255, tv2: 99, tv3: 1, tv4: 1, tv5: 1, tv6: 2, tv7: 0, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg393, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 1, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Insane),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.FastExtreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.FastExtreme),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, vg3: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.FastStandard),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.Medium),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.FastMedium),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 195, result: LamePreset.R3mix),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 196, result: LamePreset.R3mix),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 255, tv2: 99, tv3: 1, tv4: 1, tv5: 1, tv6: 2, tv7: 0, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg393, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Studio),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 192, tv2: 88, tv3: 1, tv4: 1, tv5: 1, tv6: 2, tv7: 0, result: LamePreset.CD),
            new PresetGuessRow(vg1: LameVersionGroup.lvg3931_3903up, tv1: 192, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.CD),
            new PresetGuessRow(vg1: LameVersionGroup.lvg393, tv1: 192, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.CD),
            new PresetGuessRow(vg1: LameVersionGroup.lvg394up, tv1: 192, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 195, result: LamePreset.CD),
            new PresetGuessRow(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 160, tv2: 78, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 180, result: LamePreset.Hifi)
            //new PresetGuessRow(vg1: LameVersionGroup.lvg393, vg2: LameVersionGroup.lvg3931_3903up, tv1: 160, tv2: 58, tv
        };

        IList<PresetGuessRow> actual = PresetGuesser();
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

        var actual = PresetGuesser();
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
}
