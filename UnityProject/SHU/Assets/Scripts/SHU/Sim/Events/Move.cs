using System.Diagnostics;
using SHU.Sim.Objects;
using Sim;
using ObjectId_t = System.UInt32;

namespace SHU.Sim.Events
{
    public class Move : Event
    {
        public ObjectId_t ObjectId;
        public int PreviousX;
        public int PreviousY;
        public int NewX;
        public int NewY;

        public override void Execute(Simulation sim)
        {
            var player = sim.GetObject(ObjectId) as Player;
            if (player == null)
            {
                System.Console.WriteLine("Object[{0}] is not a Walker instance.", ObjectId);
                return;
            }

            System.Console.WriteLine("ObjectId {0} ({1},{2}) -> ({3},{4})", Id, PreviousX, PreviousY, NewX, NewY);

            player.X = NewX;
            player.Y = NewY;

            player.PublishNext(this);
        }
    }
}