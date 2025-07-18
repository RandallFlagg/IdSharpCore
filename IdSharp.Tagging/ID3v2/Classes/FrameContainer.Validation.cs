using System;
using IdSharp.Common.Events;

namespace IdSharp.Tagging.ID3v2;

partial class FrameContainer
{
    private void ValidateTimeRecorded()
    {
        // TODO
    }

    private void ValidateDateRecorded()
    {
        // TODO
    }

    private void ValidateRecordingTimestamp()
    {
        var value = RecordingTimestamp;
        if (value != null)
        {
            // yyyy-MM-dd
            if (value.Length >= 10)
            {
                // MMdd
                DateRecorded = value.Substring(5, 2) + value.Substring(8, 2);
            }
            // yyyy-MM
            else if (value.Length >= 7)
            {
                // MMdd
                DateRecorded = value.Substring(6, 2) + "00";
            }
            else
            {
                DateRecorded = null;
            }

            // yyyy-MM-ddTHH:mm
            if (value.Length >= 16)
            {
                // HHmm
                TimeRecorded = value.Substring(11, 2) + value.Substring(14, 2);
            }
            // yyyy-MM-ddTHH
            else if (value.Length >= 13)
            {
                // HHmm
                TimeRecorded = value.Substring(11, 2) + "00";
            }
            else
            {
                TimeRecorded = null;
            }

            // yyyy-MM-ddTHH:mm:ss
            if (value.Length >= 19)
            {
                /* Nowhere to store seconds */
            }

            // TODO: Fire warnings on invalid data
        }
        else
        {
            DateRecorded = null;
            TimeRecorded = null;
        }
    }

    private void ValidateReleaseTimestamp()
    {
        var value = ReleaseTimestamp;
        if (value != null)
        {
            // yyyy
            if (value.Length >= 4)
            {
                Year = value.Substring(0, 4);
            }
            else
            {
                Year = null;
            }

            // TODO: Fire warnings on invalid data
        }
        else
        {
            Year = null;
        }
    }

    private void ValidateISRC()
    {
        var value = ISRC;
        if (!string.IsNullOrEmpty(value))
        {
            if (value.Length != 12)
            {
                FireWarning(nameof(ISRC), "ISRC value should be 12 characters in length");
            }
        }
    }

    private void ValidateBPM()
    {
        var value = BPM;

        if (!string.IsNullOrEmpty(value))
        {
            uint tmpBPM;
            if (!uint.TryParse(value, out tmpBPM))
            {
                FireWarning(nameof(BPM), "Value should be numeric");
            }
        }
    }

    private void ValidateTrackNumber()
    {
        // NOTE: Track Num is commonly used as a show number in the podcast world. Given where we are with podcasting as this date, 
        // many podcast shows have now exceeded 255.
        ValidateFractionValue(nameof(TrackNumber), TrackNumber, "Value should contain either the track number or track number/total tracks in the format ## or ##/##\nExample: 1 or 1/14");
    }

    private void ValidateDiscNumber()
    {
        ValidateFractionValue(nameof(DiscNumber), DiscNumber, "Value should contain either the disc number or disc number/total discs in the format ## or ##/##\nExample: 1 or 1/2");
    }

    private void ValidateFractionValue(string propertyName, string value, string message)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var isValid = true;

            var valueArray = value.Split('/');
            if (valueArray.Length > 2)
            {
                isValid = false;
            }
            else
            {
                var i = 0;
                uint extractedFirstPart = 0;
                uint extractedSecondPart = 0;
                foreach (var tmpValuePart in valueArray)
                {
                    uint tmpIntValue;
                    if (!uint.TryParse(tmpValuePart, out tmpIntValue))
                    {
                        isValid = false;
                        break;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            extractedFirstPart = tmpIntValue;
                        }
                        else if (i == 1)
                        {
                            extractedSecondPart = tmpIntValue;
                        }
                    }

                    i++;
                }

                // If first # is 0
                if (extractedFirstPart == 0)
                {
                    isValid = false;
                }
                // If ##/## used
                else if (i == 2)
                {
                    // If first partis greater than second part
                    if (extractedFirstPart > extractedSecondPart)
                    {
                        isValid = false;
                    }
                }
            }

            if (isValid == false)
            {
                FireWarning(propertyName, message);
            }
        }
    }

    private void ValidateCopyright()
    {
        var value = Copyright;

        if (!string.IsNullOrEmpty(value))
        {
            var isValid = false;
            if (value.Length >= 6)
            {
                var yearString = value.Substring(0, 4);
                int year;
                if (int.TryParse(yearString, out year) && year >= 1000 && year <= 9999)
                {
                    if (value[4] == ' ')
                    {
                        isValid = true;
                    }
                }
            }

            if (!isValid)
            {
                FireWarning(nameof(Copyright), $"The copyright field should begin with a year followed by the copyright owner{Environment.NewLine}Example: 2007 Sony Records");
            }
        }
    }

    private void ValidateYear()
    {
        var value = Year;

        if (!string.IsNullOrEmpty(value))
        {
            int tmpYear;
            if (!int.TryParse(value, out tmpYear) || tmpYear < 1000 || tmpYear >= 10000)
            {
                FireWarning(nameof(Year), $"The year field should be a 4 digit number{Environment.NewLine}Example: 2007");
            }
        }
    }

    private void ValidateUrl(string propertyName, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
            {
                FireWarning(propertyName, "Value is not a valid relative or absolute URL");
            }
        }
    }

    private void ValidatePublisherUrl()
    {
        ValidateUrl(nameof(PublisherUrl), PublisherUrl);
    }

    private void ValidateCopyrightUrl()
    {
        ValidateUrl(nameof(CopyrightUrl), CopyrightUrl);
    }

    private void ValidatePaymentUrl()
    {
        ValidateUrl(nameof(PaymentUrl), PaymentUrl);
    }

    // TODO: Add to UrlBindingList
    /*private void ValidateArtistUrl()
    {
        //ValidateUrl("ArtistUrl", this.ArtistUrl);
    }

    private void ValidateCommercialInfoUrl()
    {
        //ValidateUrl("CommercialInfoUrl", this.CommercialInfoUrl);
    }*/

    private void ValidateInternetRadioStationUrl()
    {
        ValidateUrl(nameof(InternetRadioStationUrl), InternetRadioStationUrl);
    }

    private void ValidateAudioSourceUrl()
    {
        ValidateUrl(nameof(AudioSourceUrl), AudioSourceUrl);
    }

    private void ValidateAudioFileUrl()
    {
        ValidateUrl(nameof(AudioFileUrl), AudioFileUrl);
    }

    /// <summary>
    /// Fires the InvalidData event.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="message">The message.</param>
    protected void FireWarning(string propertyName, string message)
    {
        // TODO - Add to log
        InvalidData?.Invoke(this, new InvalidDataEventArgs(propertyName, message));
    }
}
