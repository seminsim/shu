using FlatBuffers;

namespace Actors
{
    public interface IActorObserver
    {
        // the actor is just received a fake action
        void OnFeedFakeAction(uint actionId, IFlatbufferObject actionObj);
        
        // the actor is just received an action
        void OnFeedAction(uint actionId, IFlatbufferObject actionObj);

        // the actor is about to be hit
        void OnEvent(IEvent ev);
        
        // the actor is about to be destroyed in game state
        void OnDestroy();
    }
}
