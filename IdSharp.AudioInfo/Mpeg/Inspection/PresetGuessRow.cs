using System;

namespace IdSharp.AudioInfo.Mpeg.Inspection;

internal sealed class PresetGuessRow
{
    public LameVersionGroup[] VGs = new LameVersionGroup[3];  // Version Groups
    public byte[] TVs = new byte[7];                          // Tag Values
    public LamePreset Res;                                    // Resulting Lame Preset

    public PresetGuessRow(
    byte tv1, byte tv2, byte tv3, byte tv4, byte tv5, byte tv6, byte tv7,
    LamePreset result,
    LameVersionGroup vg1,
    LameVersionGroup vg2 = LameVersionGroup.None,
    LameVersionGroup vg3 = LameVersionGroup.None)
    {
        Initialize(vg1, vg2, vg3, tv1, tv2, tv3, tv4, tv5, tv6, tv7, result);
    }

    public bool HasVersionGroup(LameVersionGroup vg1)
    {
        for (var i=0; i<3; i++)
        {
            if (vg1 == VGs[i])
            {
                return true;
            }
        }

        return false;
    }

    private void Initialize(LameVersionGroup vg1, LameVersionGroup vg2, LameVersionGroup vg3,
        byte tv1, byte tv2, byte tv3, byte tv4, byte tv5, byte tv6, byte tv7,
        LamePreset result)
    {
        VGs[0] = vg1;
        VGs[1] = vg2;
        VGs[2] = vg3;
        TVs[0] = tv1;
        TVs[1] = tv2;
        TVs[2] = tv3;
        TVs[3] = tv4;
        TVs[4] = tv5;
        TVs[5] = tv6;
        TVs[6] = tv7;
        Res = result;
    }
}
