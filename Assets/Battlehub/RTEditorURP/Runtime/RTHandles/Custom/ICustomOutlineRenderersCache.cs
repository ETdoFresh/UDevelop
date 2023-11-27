using System.Collections.Generic;

namespace Battlehub.RTHandles
{
    public interface ICustomOutlineRenderersCache 
    {
        List<ICustomOutlinePrepass> GetOutlineRendererItems();
    }
}