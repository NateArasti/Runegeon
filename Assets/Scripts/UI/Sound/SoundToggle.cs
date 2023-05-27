public class SoundToggle : DoubleStateObject
{
    protected override void Start()
    {
        base.Start();

        ManualSetState(AudioSystem.SoundsEnabled);
        OnStateSet += SoundStateSet;
    }

    private void SoundStateSet(bool enabled)
    {
        AudioSystem.SetSFXState(enabled);
    }
}
