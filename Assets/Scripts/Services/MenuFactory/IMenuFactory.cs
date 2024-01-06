using BlindCrocodile.Core.Services;
using UnityEngine;

namespace BlindCrocodile.Services.MenuFactory
{
    public interface IMenuFactory : IService
    {
        GameObject CreateHud();
    }
}