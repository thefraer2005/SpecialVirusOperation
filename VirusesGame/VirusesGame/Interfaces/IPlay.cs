﻿using VirusesGame.Classes;
namespace VirusesGame.Interfaces
{
    internal interface IPlay
    {
        void Kill(Board board,int x, int y);
        void Multiply(Board board, int x, int y);
        void CancelMove();
        void SkipMove();
        string GiveUp();
    }
}