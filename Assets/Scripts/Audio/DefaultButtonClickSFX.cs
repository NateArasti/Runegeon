public class DefaultButtonClickSFX : SimpleSFXPlayer
{
    public static DefaultButtonClickSFX Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
}
