using FDK;
using TJAPlayerPI.Helper;

namespace TJAPlayerPI;

class CActSelectChangeSE : CActivity
{
    public CActSelectChangeSE(CActSelectDifficultySelect actDifficultySelect)
    {
        this.actDifficultySelect = actDifficultySelect;
    }

    public override void On活性化()
    {
        if (this.b活性化してる)
            return;

        this.donglist = new CSound[2, TJAPlayerPI.app.Skin.SECount];
        for (int nPlayer = 0; nPlayer < 2; nPlayer++)
        {
            for (int i = 0; i < TJAPlayerPI.app.Skin.SECount; i++)
            {
                CSound? cSound = TJAPlayerPI.SoundManager.tCreateSound(CSkin.Path(@"Sounds/Taiko/" + i.ToString() + @"/dong.ogg"), ESoundGroup.SoundEffect);
                this.donglist[nPlayer, i] = cSound;
                if (TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount >= 2 && TJAPlayerPI.app.ConfigToml.PlayOption.UsePanning && cSound is not null)
                    cSound.nPanning = (nPlayer * 200) - 100;
            }
        }

        this.SEName = new CTexture[2];
        this.NameMoving = new CTexture[2];
        this.SENameList = new CTexture[TJAPlayerPI.app.Skin.SECount];

        using (var font = HFontHelper.tCreateFont(30))
            for (int i = 0; i < TJAPlayerPI.app.Skin.SECount; i++)
            {
                string SEName = "Untitled";
                if (i < TJAPlayerPI.app.Skin.SkinConfig.Sound.SENames.Length)
                    SEName = TJAPlayerPI.app.Skin.SkinConfig.Sound.SENames[i];
                using (var bmp = font.DrawText(SEName, Color.White, Color.Black, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                    this.SENameList[i] = TJAPlayerPI.app.tCreateTexture(bmp);
            }

        this.SENameChanger(0);
        this.SENameChanger(1);

        base.On活性化();
    }
    public override void On非活性化()
    {
        if (this.b活性化してない)
            return;

        for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            for (int i = 0; i < TJAPlayerPI.app.Skin.SECount; i++)
                this.donglist[nPlayer, i]?.t解放する();

        //Classは参照渡しであるため、ListをDisposeするだけでよい
        TJAPlayerPI.t安全にDisposeする(ref this.SENameList);

        base.On非活性化();
    }

    public override int On進行描画()
    {
        if (this.b活性化してない)
            return 0;

        #region [ 初めての進行描画 ]
        //-----------------
        if (this.b初めての進行描画)
        {
            base.b初めての進行描画 = false;
        }
        //-----------------
        #endregion

        for (int nPlayer = 0; nPlayer < 2; nPlayer++)
        {
            this.ct登場退場アニメ用[nPlayer].t進行();
            if (this.ePhase[nPlayer] == EChangeSEPhase.Active)
            {
                this.boxたちの描画(TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxX[nPlayer], TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxY[nPlayer], nPlayer);
            }
            else if (this.ePhase[nPlayer] == EChangeSEPhase.AnimationIn)
            {
                int y = (int)((TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxY[nPlayer]) + (float)((Math.Sin(this.ct登場退場アニメ用[nPlayer].n現在の値 / 100.0) - 0.9) * -500f));
                this.boxたちの描画(TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxX[nPlayer], y, nPlayer);

                if (this.ct登場退場アニメ用[nPlayer].b終了値に達した)
                    this.ePhase[nPlayer] = EChangeSEPhase.Active;
            }
            else if (this.ePhase[nPlayer] == EChangeSEPhase.AnimationOut)
            {
                int y = (int)((TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxY[nPlayer]) + (float)((Math.Sin((this.ct登場退場アニメ用[nPlayer].n終了値 - this.ct登場退場アニメ用[nPlayer].n現在の値) / 100.0) - 0.9) * -500f));
                this.boxたちの描画(TJAPlayerPI.app.Skin.SkinConfig.SongSelect.Difficulty.ChangeSEBoxX[nPlayer], y, nPlayer);

                if (this.ct登場退場アニメ用[nPlayer].b終了値に達した)
                    this.ePhase[nPlayer] = EChangeSEPhase.Inactive;
            }
        }

        if (this.ePhase[0] == EChangeSEPhase.Active)
        {
            if (TJAPlayerPI.app.Pad.bPressed(EPad.LRed) || TJAPlayerPI.app.Pad.bPressed(EPad.RRed) || TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.Return))
            {
                TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND決定音]?.t再生する();
                this.tDeativateChangeSE(0);
            }
            if ((TJAPlayerPI.app.Pad.bPressed(EPad.LBlue) || TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.LeftArrow)) && TJAPlayerPI.app.Skin.SECount != 0)
            {
                TJAPlayerPI.app.Skin.NowSENum[0]--;
                if (TJAPlayerPI.app.Skin.NowSENum[0] < 0)
                    TJAPlayerPI.app.Skin.NowSENum[0] = TJAPlayerPI.app.Skin.SECount - 1;
                this.donglist[0, TJAPlayerPI.app.Skin.NowSENum[0]]?.t再生を開始する();
                this.MoveStart(EMoving.LeftMoving, 0);
                this.SENameChanger(0);
            }
            if ((TJAPlayerPI.app.Pad.bPressed(EPad.RBlue) || TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.RightArrow)) && TJAPlayerPI.app.Skin.SECount != 0)
            {
                TJAPlayerPI.app.Skin.NowSENum[0]++;
                if (TJAPlayerPI.app.Skin.NowSENum[0] > TJAPlayerPI.app.Skin.SECount - 1)
                    TJAPlayerPI.app.Skin.NowSENum[0] = 0;
                this.donglist[0, TJAPlayerPI.app.Skin.NowSENum[0]]?.t再生を開始する();
                this.MoveStart(EMoving.RightMoving, 0);
                this.SENameChanger(0);
            }
        }
        if (this.ePhase[1] == EChangeSEPhase.Active)
        {
            if (TJAPlayerPI.app.Pad.bPressed(EPad.LRed2P) || TJAPlayerPI.app.Pad.bPressed(EPad.RRed2P) || (TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.Return) && actDifficultySelect.選択済み[0]))
            {
                TJAPlayerPI.app.Skin.SystemSounds[Eシステムサウンド.SOUND決定音]?.t再生する();
                this.tDeativateChangeSE(1);
            }
            if ((TJAPlayerPI.app.Pad.bPressed(EPad.LBlue2P) || (TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.LeftArrow) && actDifficultySelect.選択済み[0])) && TJAPlayerPI.app.Skin.SECount != 0)
            {
                TJAPlayerPI.app.Skin.NowSENum[1]--;
                if (TJAPlayerPI.app.Skin.NowSENum[1] < 0)
                    TJAPlayerPI.app.Skin.NowSENum[1] = TJAPlayerPI.app.Skin.SECount - 1;
                this.donglist[1, TJAPlayerPI.app.Skin.NowSENum[1]]?.t再生を開始する();
                this.MoveStart(EMoving.LeftMoving, 1);
                this.SENameChanger(1);
            }
            if ((TJAPlayerPI.app.Pad.bPressed(EPad.RBlue2P) || (TJAPlayerPI.app.InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.RightArrow) && actDifficultySelect.選択済み[0])) && TJAPlayerPI.app.Skin.SECount != 0)
            {
                TJAPlayerPI.app.Skin.NowSENum[1]++;
                if (TJAPlayerPI.app.Skin.NowSENum[1] > TJAPlayerPI.app.Skin.SECount - 1)
                    TJAPlayerPI.app.Skin.NowSENum[1] = 0;
                this.donglist[1, TJAPlayerPI.app.Skin.NowSENum[1]]?.t再生を開始する();
                this.MoveStart(EMoving.RightMoving, 1);
                this.SENameChanger(1);
            }
        }

        return base.On進行描画();
    }

    private void MoveStart(EMoving lr, int nPlayer)
    {
        this.eMoving[nPlayer] = lr;
        this.ct変更アニメ用[nPlayer].t時間Reset();
        this.ct変更アニメ用[nPlayer].n現在の値 = 0;

    }

    private void boxたちの描画(int x, int y, int nPlayer)
    {
        if (TJAPlayerPI.app.Tx.ChangeSE_Box is not null)
            TJAPlayerPI.app.Tx.ChangeSE_Box.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x, y);

        if (TJAPlayerPI.app.Tx.ChangeSE_Num is not null)
        {
            for (int i = 0; i < TJAPlayerPI.app.Skin.SECount.ToString().Length; i++)
            {
                var number = (int)(TJAPlayerPI.app.Skin.SECount / Math.Pow(10, TJAPlayerPI.app.Skin.SECount.ToString().Length - i - 1) % 10);
                Rectangle rectangle = new Rectangle(TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10 * number, 0, TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Height);
                TJAPlayerPI.app.Tx.ChangeSE_Num.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (i * TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10) + 20, y - 260, rectangle);
            }
            for (int i = 0; i < (TJAPlayerPI.app.Skin.NowSENum[nPlayer] + 1).ToString().Length; i++)
            {
                var number = (int)((TJAPlayerPI.app.Skin.NowSENum[nPlayer] + 1) / Math.Pow(10, i) % 10);
                Rectangle rectangle = new Rectangle(TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10 * number, 0, TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10, TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Height);
                TJAPlayerPI.app.Tx.ChangeSE_Num.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x - (i * TJAPlayerPI.app.Tx.ChangeSE_Num.szTextureSize.Width / 10) - 20, y - 260, rectangle);
            }
        }
        CTexture? _SEName = this.SEName[nPlayer];
        CTexture? _NameMoving = this.NameMoving[nPlayer];
        if (eMoving[nPlayer] == EMoving.None)
        {
            if (TJAPlayerPI.app.Tx.ChangeSE_Note is not null)
            {
                TJAPlayerPI.app.Tx.ChangeSE_Note.Opacity = 0xff;
                TJAPlayerPI.app.Tx.ChangeSE_Note.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x, y);
            }
            if (_SEName is not null)
            {
                _SEName.Opacity = 0xff;
                _SEName.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x, y - 50);
            }
        }
        else if (eMoving[nPlayer] == EMoving.LeftMoving)
        {
            this.ct変更アニメ用[nPlayer].t進行();
            if (TJAPlayerPI.app.Tx.ChangeSE_Note is not null)
            {
                TJAPlayerPI.app.Tx.ChangeSE_Note.Opacity = ct変更アニメ用[nPlayer].n現在の値;
                TJAPlayerPI.app.Tx.ChangeSE_Note.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (int)((ct変更アニメ用[nPlayer].n現在の値 - ct変更アニメ用[nPlayer].n終了値) * 0.5f), y);
                TJAPlayerPI.app.Tx.ChangeSE_Note.Opacity = ct変更アニメ用[nPlayer].n終了値 - ct変更アニメ用[nPlayer].n現在の値;
                TJAPlayerPI.app.Tx.ChangeSE_Note.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (int)((ct変更アニメ用[nPlayer].n現在の値) * 0.5f), y);
            }
            if (_NameMoving is not null)
            {
                _NameMoving.Opacity = ct変更アニメ用[nPlayer].n終了値 - ct変更アニメ用[nPlayer].n現在の値;
                _NameMoving?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (int)((ct変更アニメ用[nPlayer].n現在の値) * 0.5f), y - 50);
            }
            if (_SEName is not null)
            {
                _SEName.Opacity = ct変更アニメ用[nPlayer].n現在の値;
                _SEName?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x + (int)((ct変更アニメ用[nPlayer].n現在の値 - ct変更アニメ用[nPlayer].n終了値) * 0.5f), y - 50);
            }
            if (this.ct変更アニメ用[nPlayer].b終了値に達した)
                this.eMoving[nPlayer] = EMoving.None;
        }
        else
        {
            this.ct変更アニメ用[nPlayer].t進行();
            if (TJAPlayerPI.app.Tx.ChangeSE_Note is not null)
            {
                TJAPlayerPI.app.Tx.ChangeSE_Note.Opacity = ct変更アニメ用[nPlayer].n現在の値;
                TJAPlayerPI.app.Tx.ChangeSE_Note.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x - (int)((ct変更アニメ用[nPlayer].n現在の値 - ct変更アニメ用[nPlayer].n終了値) * 0.5f), y);
                TJAPlayerPI.app.Tx.ChangeSE_Note.Opacity = ct変更アニメ用[nPlayer].n終了値 - ct変更アニメ用[nPlayer].n現在の値;
                TJAPlayerPI.app.Tx.ChangeSE_Note.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x - (int)((ct変更アニメ用[nPlayer].n現在の値) * 0.5f), y);
            }
            if (_NameMoving is not null)
            {
                _NameMoving.Opacity = ct変更アニメ用[nPlayer].n終了値 - ct変更アニメ用[nPlayer].n現在の値;
                _NameMoving?.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x - (int)((ct変更アニメ用[nPlayer].n現在の値) * 0.5f), y - 50);
            }
            if (_SEName is not null)
            {
                _SEName.Opacity = ct変更アニメ用[nPlayer].n現在の値;
                _SEName.t2D拡大率考慮描画(TJAPlayerPI.app.Device, CTexture.RefPnt.Down, x - (int)((ct変更アニメ用[nPlayer].n現在の値 - ct変更アニメ用[nPlayer].n終了値) * 0.5f), y - 50);
            }
            if (this.ct変更アニメ用[nPlayer].b終了値に達した)
                this.eMoving[nPlayer] = EMoving.None;
        }
    }

    private void SENameChanger(int nPlayer)
    {
        if (TJAPlayerPI.app.Skin.SECount != 0)
        {
            this.NameMoving[nPlayer] = this.SEName[nPlayer];
            this.SEName[nPlayer] = this.SENameList[TJAPlayerPI.app.Skin.NowSENum[nPlayer]];
        }
    }

    /// <summary>
    /// 音色切り替えをアクティブ化
    /// </summary>
    /// <param name="nPlayer">プレイヤー番号</param>
    public void tActivateChangeSE(int nPlayer)
    {
        if (TJAPlayerPI.app.Skin.SECount != 0 && ePhase[nPlayer] == EChangeSEPhase.Inactive)
        {
            ePhase[nPlayer] = EChangeSEPhase.AnimationIn;
            ct登場退場アニメ用[nPlayer].t時間Reset();
            ct登場退場アニメ用[nPlayer].n現在の値 = 0;
        }
    }

    /// <summary>
    /// 音色切り替えを非アクティブ化
    /// </summary>
    /// <param name="nPlayer">プレイヤー番号</param>
    public void tDeativateChangeSE(int nPlayer)
    {
        if (ePhase[nPlayer] == EChangeSEPhase.Active)
        {
            ePhase[nPlayer] = EChangeSEPhase.AnimationOut;
            ct登場退場アニメ用[nPlayer].t時間Reset();
            ct登場退場アニメ用[nPlayer].n現在の値 = 0;
        }
    }


    #region[private]
    private EChangeSEPhase[] ePhase = { EChangeSEPhase.Inactive, EChangeSEPhase.Inactive };

    private EMoving[] eMoving = { EMoving.None, EMoving.None };

    public bool[] bIsActive
    {
        get
        {
            return [ePhase[0] != EChangeSEPhase.Inactive, ePhase[1] != EChangeSEPhase.Inactive];
        }
    }

    private CCounter[] ct登場退場アニメ用 = { new CCounter(0, 203, 2, TJAPlayerPI.app.Timer), new CCounter(0, 203, 2, TJAPlayerPI.app.Timer) };
    private CCounter[] ct変更アニメ用 = { new CCounter(0, 255, 1, TJAPlayerPI.app.Timer), new CCounter(0, 255, 1, TJAPlayerPI.app.Timer) };

    private enum EMoving
    {
        None,
        LeftMoving,
        RightMoving
    }

    private enum EChangeSEPhase
    {
        Inactive,
        AnimationIn,
        Active,
        AnimationOut
    }

    private CSound?[,] donglist;
    private CTexture?[] SEName;
    private CTexture?[] NameMoving;
    private CTexture?[] SENameList;
    private CActSelectDifficultySelect actDifficultySelect;
    #endregion
}
