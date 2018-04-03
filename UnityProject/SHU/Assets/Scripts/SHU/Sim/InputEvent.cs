using FlatBuffers;
using SHU.Sim;

namespace Sim
 {
     public abstract class InputEvent<T> : Event
         where T : IFlatbufferObject
     {
         public T Fbo;    // FlatBufferObject

         protected InputEvent(T fbo)
         {
             Fbo = fbo;
         }
     }
 }