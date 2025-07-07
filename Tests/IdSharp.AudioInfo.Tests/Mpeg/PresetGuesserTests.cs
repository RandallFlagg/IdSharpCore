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
            new(vg1: LameVersionGroup.lvg390_3901_392, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Insane),
            new(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Insane),
            new(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 1, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Insane),
            new(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.Extreme),
            new(vg1: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.Extreme),
            new(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.FastExtreme),
            new(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.FastExtreme),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 0, tv2: 78, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 3, tv4: 1, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.Standard),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, vg3: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 78, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 190, result: LamePreset.FastStandard),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 3, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.Medium),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 68, tv3: 4, tv4: 2, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.FastMedium),
            new(vg1: LameVersionGroup.lvg390_3901_392, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 195, result: LamePreset.R3mix),
            new(vg1: LameVersionGroup.lvg3902_391, vg2: LameVersionGroup.lvg3931_3903up, tv1: 0, tv2: 88, tv3: 4, tv4: 1, tv5: 3, tv6: 3, tv7: 196, result: LamePreset.R3mix),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 255, tv2: 99, tv3: 1, tv4: 1, tv5: 1, tv6: 2, tv7: 0, result: LamePreset.Studio),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 206, result: LamePreset.Studio),
            new(vg1: LameVersionGroup.lvg393, tv1: 255, tv2: 58, tv3: 2, tv4: 1, tv5: 3, tv6: 2, tv7: 205, result: LamePreset.Studio),
            new(vg1: LameVersionGroup.lvg394up, tv1: 255, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 205, result: LamePreset.Studio),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 192, tv2: 88, tv3: 1, tv4: 1, tv5: 1, tv6: 2, tv7: 0, result: LamePreset.CD),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 192, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 196, result: LamePreset.CD),
            new(vg1: LameVersionGroup.lvg393, tv1: 192, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 195, result: LamePreset.CD),
            new(vg1: LameVersionGroup.lvg394up, tv1: 192, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 195, result: LamePreset.CD),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 160, tv2: 78, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 180, result: LamePreset.Hifi),
            new(vg1: LameVersionGroup.lvg393, vg2: LameVersionGroup.lvg3931_3903up, tv1: 160, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 180, result: LamePreset.Hifi),
            new(vg1: LameVersionGroup.lvg394up, tv1: 160, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 180, result: LamePreset.Hifi),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 128, tv2: 67, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 180, result: LamePreset.Tape),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 128, tv2: 67, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 150, result: LamePreset.Radio),
            new(vg1: LameVersionGroup.lvg390_3901_392, vg2: LameVersionGroup.lvg3902_391, tv1: 112, tv2: 67, tv3: 1, tv4: 1, tv5: 3, tv6: 2, tv7: 150, result: LamePreset.FM),
            new(vg1: LameVersionGroup.lvg393, vg2: LameVersionGroup.lvg3931_3903up, tv1: 112, tv2: 58, tv3: 2, tv4: 2, tv5: 3, tv6: 2, tv7: 160, result: LamePreset.TapeRadioFM),
            new(vg1: LameVersionGroup.lvg394up, tv1: 112, tv2: 57, tv3: 2, tv4: 1, tv5: 3, tv6: 4, tv7: 160, result: LamePreset.TapeRadioFM),
            new(vg1: LameVersionGroup.lvg393, vg2: LameVersionGroup.lvg3931_3903up, tv1: 56, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 100, result: LamePreset.Voice),
            new(vg1: LameVersionGroup.lvg394up, tv1: 56, tv2: 57, tv3: 2, tv4: 1, tv5: 0, tv6: 4, tv7: 150, result: LamePreset.Voice),
            new(vg1: LameVersionGroup.lvg390_3901_392, tv1: 40, tv2: 65, tv3: 1, tv4: 1, tv5: 0, tv6: 2, tv7: 75, result: LamePreset.MWUS),
            new(vg1: LameVersionGroup.lvg3902_391, tv1: 40, tv2: 65, tv3: 1, tv4: 1, tv5: 0, tv6: 2, tv7: 76, result: LamePreset.MWUS),
            new(vg1: LameVersionGroup.lvg393, vg2: LameVersionGroup.lvg3931_3903up, tv1: 40, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 70, result: LamePreset.MWUS),
            new(vg1: LameVersionGroup.lvg394up, tv1: 40, tv2: 57, tv3: 2, tv4: 1, tv5: 0, tv6: 4, tv7: 105, result: LamePreset.MWUS),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 24, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 40, result: LamePreset.MWEU),
            new(vg1: LameVersionGroup.lvg393, tv1: 24, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 39, result: LamePreset.MWEU),
            new(vg1: LameVersionGroup.lvg394up, tv1: 24, tv2: 57, tv3: 2, tv4: 1, tv5: 0, tv6: 4, tv7: 59, result: LamePreset.MWEU),
            new(vg1: LameVersionGroup.lvg3931_3903up, tv1: 16, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 38, result: LamePreset.Phone),
            new(vg1: LameVersionGroup.lvg393, tv1: 16, tv2: 58, tv3: 2, tv4: 2, tv5: 0, tv6: 2, tv7: 37, result: LamePreset.Phone),
            new(vg1: LameVersionGroup.lvg394up, tv1: 16, tv2: 57, tv3: 2, tv4: 1, tv5: 0, tv6: 4, tv7: 56, result: LamePreset.Phone)
        };

        //IList<PresetGuessRow> actual = PresetGuesser();
        List<PresetGuessRow> actual = null; // Replace with actual call to PresetGuesser() method
        Assert.That(actual.Count, Is.EqualTo(expected.Count), "Preset row count mismatch");

        for (var i = 0; i < expected.Count; i++)
        {
            var a = actual[i];
            var e = expected[i];

            Assert.That(a.Res, Is.EqualTo(e.Res), $"Row {i} - Res mismatch");

            for (var t = 0; t < 7; t++)
            {
                Assert.That(a.TVs[t], Is.EqualTo(e.TVs[t]), $"Row {i} - TVs[{t}] mismatch");
            }

            for (var v = 0; v < 3; v++)
            {
                Assert.That(a.VGs[v], Is.EqualTo(e.VGs[v]), $"Row {i} - VGs[{v}] mismatch");
            }
        }


        Assert.That(actual.Count, Is.EqualTo(expected.Count), "PresetGuesser entry count mismatch");

        for (var i = 0; i < expected.Count; i++)
        {
            var act = actual[i];
            var exp = expected[i];

            var msg = $"Row {i}: ";

            Assert.That(act.TVs[0], Is.EqualTo(exp.TVs[0]), msg + "TV1 mismatch");
            Assert.That(act.TVs[1], Is.EqualTo(exp.TVs[1]), msg + "TV2 mismatch");
            Assert.That(act.TVs[2], Is.EqualTo(exp.TVs[2]), msg + "TV3 mismatch");
            Assert.That(act.TVs[3], Is.EqualTo(exp.TVs[3]), msg + "TV4 mismatch");
            Assert.That(act.TVs[4], Is.EqualTo(exp.TVs[4]), msg + "TV5 mismatch");
            Assert.That(act.TVs[5], Is.EqualTo(exp.TVs[5]), msg + "TV6 mismatch");
            Assert.That(act.TVs[6], Is.EqualTo(exp.TVs[6]), msg + "TV7 mismatch");

            Assert.That(act.Res, Is.EqualTo(exp.Res), msg + "Preset mismatch");

            Assert.That(act.VGs[0], Is.EqualTo(exp.VGs[0]), msg + "VG1 mismatch");
            var vg1 = exp.VGs[1] != LameVersionGroup.None ? exp.VGs[1] : LameVersionGroup.None;
            Assert.That(act.VGs[1], Is.EqualTo(vg1), msg + "VG2 mismatch");
            var vg2 = exp.VGs[2] != LameVersionGroup.None ? exp.VGs[2] : LameVersionGroup.None;
            Assert.That(act.VGs[2], Is.EqualTo(vg2), msg + "VG3 mismatch");
        }
    }
}
