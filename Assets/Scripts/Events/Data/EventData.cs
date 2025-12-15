namespace Events.Data
{
    public class DamageEventData
    {
        public int DamageAmount;
        public bool IsPlayer;

        public void Reset()
        {
            DamageAmount = 0;
            IsPlayer = false;
        }
    }
}
