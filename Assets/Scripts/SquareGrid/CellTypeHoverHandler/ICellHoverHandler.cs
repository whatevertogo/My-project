using UnityEngine;

    public interface ICellHoverHandler
    {
        void OnHoverEnter(SquareCell cell);
        void OnHoverExit(SquareCell cell);
    }
