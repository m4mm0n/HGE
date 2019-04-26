namespace HGE.IO.AudioEngine.BASSlib
{
    public enum BASSError
    {
        /// <summary>
        /// All is OK
        /// </summary>
        // Token: 0x0400025B RID: 603
        BASS_OK,
        /// <summary>
        /// Memory error
        /// </summary>
        // Token: 0x0400025C RID: 604
        BASS_ERROR_MEM,
        /// <summary>
        /// Can't open the file
        /// </summary>
        // Token: 0x0400025D RID: 605
        BASS_ERROR_FILEOPEN,
        /// <summary>
        /// Can't find a free/valid driver
        /// </summary>
        // Token: 0x0400025E RID: 606
        BASS_ERROR_DRIVER,
        /// <summary>
        /// The sample buffer was lost
        /// </summary>
        // Token: 0x0400025F RID: 607
        BASS_ERROR_BUFLOST,
        /// <summary>
        /// Invalid handle
        /// </summary>
        // Token: 0x04000260 RID: 608
        BASS_ERROR_HANDLE,
        /// <summary>
        /// Unsupported sample format
        /// </summary>
        // Token: 0x04000261 RID: 609
        BASS_ERROR_FORMAT,
        /// <summary>
        /// Invalid playback position
        /// </summary>
        // Token: 0x04000262 RID: 610
        BASS_ERROR_POSITION,
        /// <summary>
        /// BASS_Init has not been successfully called
        /// </summary>
        // Token: 0x04000263 RID: 611
        BASS_ERROR_INIT,
        /// <summary>
        /// BASS_Start has not been successfully called
        /// </summary>
        // Token: 0x04000264 RID: 612
        BASS_ERROR_START,
        /// <summary>
        /// No CD in drive
        /// </summary>
        // Token: 0x04000265 RID: 613
        BASS_ERROR_NOCD = 12,
        /// <summary>
        /// Invalid track number
        /// </summary>
        // Token: 0x04000266 RID: 614
        BASS_ERROR_CDTRACK,
        /// <summary>
        /// Already initialized/paused/whatever
        /// </summary>
        // Token: 0x04000267 RID: 615
        BASS_ERROR_ALREADY,
        /// <summary>
        /// Not paused
        /// </summary>
        // Token: 0x04000268 RID: 616
        BASS_ERROR_NOPAUSE = 16,
        /// <summary>
        /// Not an audio track
        /// </summary>
        // Token: 0x04000269 RID: 617
        BASS_ERROR_NOTAUDIO,
        /// <summary>
        /// Can't get a free channel
        /// </summary>
        // Token: 0x0400026A RID: 618
        BASS_ERROR_NOCHAN,
        /// <summary>
        /// An illegal type was specified
        /// </summary>
        // Token: 0x0400026B RID: 619
        BASS_ERROR_ILLTYPE,
        /// <summary>
        /// An illegal parameter was specified
        /// </summary>
        // Token: 0x0400026C RID: 620
        BASS_ERROR_ILLPARAM,
        /// <summary>
        /// No 3D support
        /// </summary>
        // Token: 0x0400026D RID: 621
        BASS_ERROR_NO3D,
        /// <summary>
        /// No EAX support
        /// </summary>
        // Token: 0x0400026E RID: 622
        BASS_ERROR_NOEAX,
        /// <summary>
        /// Illegal device number
        /// </summary>
        // Token: 0x0400026F RID: 623
        BASS_ERROR_DEVICE,
        /// <summary>
        /// Not playing
        /// </summary>
        // Token: 0x04000270 RID: 624
        BASS_ERROR_NOPLAY,
        /// <summary>
        /// Illegal sample rate
        /// </summary>
        // Token: 0x04000271 RID: 625
        BASS_ERROR_FREQ,
        /// <summary>
        /// The stream is not a file stream
        /// </summary>
        // Token: 0x04000272 RID: 626
        BASS_ERROR_NOTFILE = 27,
        /// <summary>
        /// No hardware voices available
        /// </summary>
        // Token: 0x04000273 RID: 627
        BASS_ERROR_NOHW = 29,
        /// <summary>
        /// The MOD music has no sequence data
        /// </summary>
        // Token: 0x04000274 RID: 628
        BASS_ERROR_EMPTY = 31,
        /// <summary>
        /// No internet connection could be opened
        /// </summary>
        // Token: 0x04000275 RID: 629
        BASS_ERROR_NONET,
        /// <summary>
        /// Couldn't create the file
        /// </summary>
        // Token: 0x04000276 RID: 630
        BASS_ERROR_CREATE,
        /// <summary>
        /// Effects are not available
        /// </summary>
        // Token: 0x04000277 RID: 631
        BASS_ERROR_NOFX,
        /// <summary>
        /// The channel is playing
        /// </summary>
        // Token: 0x04000278 RID: 632
        BASS_ERROR_PLAYING,
        /// <summary>
        /// Requested data is not available
        /// </summary>
        // Token: 0x04000279 RID: 633
        BASS_ERROR_NOTAVAIL = 37,
        /// <summary>
        /// The channel is a 'decoding channel'
        /// </summary>
        // Token: 0x0400027A RID: 634
        BASS_ERROR_DECODE,
        /// <summary>
        /// A sufficient DirectX version is not installed
        /// </summary>
        // Token: 0x0400027B RID: 635
        BASS_ERROR_DX,
        /// <summary>
        /// Connection timedout
        /// </summary>
        // Token: 0x0400027C RID: 636
        BASS_ERROR_TIMEOUT,
        /// <summary>
        /// Unsupported file format
        /// </summary>
        // Token: 0x0400027D RID: 637
        BASS_ERROR_FILEFORM,
        /// <summary>
        /// Unavailable speaker
        /// </summary>
        // Token: 0x0400027E RID: 638
        BASS_ERROR_SPEAKER,
        /// <summary>
        /// Invalid BASS version (used by add-ons)
        /// </summary>
        // Token: 0x0400027F RID: 639
        BASS_ERROR_VERSION,
        /// <summary>
        /// Codec is not available/supported
        /// </summary>
        // Token: 0x04000280 RID: 640
        BASS_ERROR_CODEC,
        /// <summary>
        /// The channel/file has ended
        /// </summary>
        // Token: 0x04000281 RID: 641
        BASS_ERROR_ENDED,
        /// <summary>
        /// The device is busy (eg. in "exclusive" use by another process)
        /// </summary>
        // Token: 0x04000282 RID: 642
        BASS_ERROR_BUSY,
        /// <summary>
        /// Some other mystery error
        /// </summary>
        // Token: 0x04000283 RID: 643
        BASS_ERROR_UNKNOWN = -1,
        /// <summary>
        /// BassWma: the file is protected
        /// </summary>
        // Token: 0x04000284 RID: 644
        BASS_ERROR_WMA_LICENSE = 1000,
        /// <summary>
        /// BassWma: WM9 is required
        /// </summary>
        // Token: 0x04000285 RID: 645
        BASS_ERROR_WMA_WM9,
        /// <summary>
        /// BassWma: access denied (user/pass is invalid)
        /// </summary>
        // Token: 0x04000286 RID: 646
        BASS_ERROR_WMA_DENIED,
        /// <summary>
        /// BassWma: no appropriate codec is installed
        /// </summary>
        // Token: 0x04000287 RID: 647
        BASS_ERROR_WMA_CODEC,
        /// <summary>
        /// BassWma: individualization is needed
        /// </summary>
        // Token: 0x04000288 RID: 648
        BASS_ERROR_WMA_INDIVIDUAL,
        /// <summary>
        /// BassEnc: ACM codec selection cancelled
        /// </summary>
        // Token: 0x04000289 RID: 649
        BASS_ERROR_ACM_CANCEL = 2000,
        /// <summary>
        /// BassEnc: Access denied (invalid password)
        /// </summary>
        // Token: 0x0400028A RID: 650
        BASS_ERROR_CAST_DENIED = 2100,
        /// <summary>
        /// BassVst: the given effect has no inputs and is probably a VST instrument and no effect
        /// </summary>
        // Token: 0x0400028B RID: 651
        BASS_VST_ERROR_NOINPUTS = 3000,
        /// <summary>
        /// BassVst:  the given effect has no outputs
        /// </summary>
        // Token: 0x0400028C RID: 652
        BASS_VST_ERROR_NOOUTPUTS,
        /// <summary>
        /// BassVst: the given effect does not support realtime processing
        /// </summary>
        // Token: 0x0400028D RID: 653
        BASS_VST_ERROR_NOREALTIME,
        /// <summary>
        /// BASSWASAPI: no WASAPI available
        /// </summary>
        // Token: 0x0400028E RID: 654
        BASS_ERROR_WASAPI = 5000,
        /// <summary>
        /// BASS_AAC: non-streamable due to MP4 atom order ('mdat' before 'moov')
        /// </summary>
        // Token: 0x0400028F RID: 655
        BASS_ERROR_MP4_NOSTREAM = 6000
    }
}
