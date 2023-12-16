using BlindCrocodile.Core.Services;
using UnityEngine;

namespace BlindCrocodile.Services.MenyFactory
{
    public interface IMenuFactory : IService
    {
        GameObject CreateHud();
    }
}