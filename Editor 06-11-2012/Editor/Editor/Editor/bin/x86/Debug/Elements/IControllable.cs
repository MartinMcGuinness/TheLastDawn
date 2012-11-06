using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.Elements
{
    public interface IControllable
    {
        void AActionStart();
        void AAction();
        void AActionStop();
        void BActionStart();
        void BAction();
        void BActionStop();
        void XActionStart();
        void XAction();
        void XActionStop();
        void YActionStart();
        void YAction();
        void YActionStop();
        void RightActionStart();
        void RightAction();
        void RightActionStop();
        void LeftActionStart();
        void LeftAction();
        void LeftActionStop();
        void UpActionStart();
        void UpAction();
        void UpActionStop();
        void DownActionStart();
        void DownAction();
        void DownActionStop();
    }
}