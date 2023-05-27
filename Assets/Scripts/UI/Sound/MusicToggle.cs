public class MusicToggle : DoubleStateObject
{
    protected override void Start()
    {
        base.Start();

        ManualSetState(AudioSystem.SoundsEnabled);
        OnStateSet += MusicStateSet;
    }

    private void MusicStateSet(bool enabled)
    {
        AudioSystem.SetMusicState(enabled);
    }
}
