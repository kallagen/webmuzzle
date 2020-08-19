namespace TSensor.FakeSensor
{
    public enum State : int
    {
        Nothing = 0,
        TankDec = 1,
        TankWaitTanker = 2,
        TankInc = 3,
        TankerInc = 4,
        TankerMoveToTank = 5,
        TankerDec = 6,
        TankerMoveToStorage = 7,
        StorageDec = 8
    }
}
