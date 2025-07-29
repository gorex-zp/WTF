using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace WtfApp.GUI
{
    public class GUIContainer: GuiObject
    {
        public enum GUIDirection { HORIZONTAL, VERTICAL }
        public enum GUITypePosition { AUTO_POS_AUTO_SIZE, AUTO_POS_USER_SIZE, USER_POS_USER_SIZE }
        public enum ContentHAlign { LEFT, CENTER , RIGHT, AUTOINTERVAL, FILL }
        public enum ContentVAlign { TOP, CENTER, BOTTOM, AUTOINTERVAL, FILL }
        public enum ContentMultiLine { POSSIBLE, IMPOSSIBLE }

        public Dictionary<string, GuiObject> guiObjects = new Dictionary<string, GuiObject>();
        private Engine.Scene.Scene _parent;
        public Color backColor;
        private GUIDirection guiDirection;
        private GUITypePosition guiTypePosition;
        public ContentHAlign contentHorizontalAlign;
        public ContentVAlign contentVerticalAlign;
        public ContentMultiLine contentMultiLine;
        public int guiInterval;
        public int preferColCount = 0;
        public int preferRowCount = 0;
        

        private bool leftToRight = true;
        private bool topToBottom = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerArea"></param>
        /// <param name="backColor"></param>
        /// <param name="guiDirection"></param>
        /// <param name="guiTypePosition"></param>
        /// <param name="guiInterval">-1 = Auto</param>
        public GUIContainer(string name,Engine.Scene.Scene scene,Rectangle containerArea, Color backColor, GUIDirection guiDirection, GUITypePosition guiTypePosition, int guiInterval):base(name,GuiObjectType.CONTAINER)
        {
            this._parent = scene;
            this._rectangle = containerArea;
            this.backColor = backColor;
            this.guiDirection = guiDirection;
            this.guiTypePosition = guiTypePosition;
            this.guiInterval = guiInterval;

            contentHorizontalAlign = ContentHAlign.CENTER;
            contentVerticalAlign = ContentVAlign.CENTER;
            contentMultiLine = ContentMultiLine.POSSIBLE;

            borderStyle = BorderStyle.NONE;
            borderWidth = 1;
            borderColor = Color.Black;
        }

        public bool AddGuiObj(GuiObject guiObject)
        {
            if(TryAddGuiObj(guiObject))
            {
                guiObjects.Add(guiObject.Name, guiObject);
                ResetChildObjPosition(guiObject);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetChildObjPosition(GuiObject newItem = null)
        {
            switch (guiTypePosition)
            {
                case GUITypePosition.AUTO_POS_AUTO_SIZE:
                    if (guiDirection == GUIDirection.HORIZONTAL)
                    {
                        if(contentMultiLine==ContentMultiLine.IMPOSSIBLE)
                        {
                            int cellSize = (_rectangle.Width > _rectangle.Height ? _rectangle.Height : _rectangle.Width) - guiInterval*2;
                            if (cellSize < 1)
                                cellSize = 0;
                            //get cellsize for one row
                            float hCof = ((float)(_rectangle.Width - ((guiObjects.Count * guiInterval)+ guiInterval)) / ((float)(guiObjects.Count * cellSize)));
                            if(hCof<1)
                                cellSize = (int)Math.Floor((cellSize * (hCof + 0.001)));

                            for (int i=0;i<guiObjects.Count;i++)
                            {
                                switch (contentHorizontalAlign)
                                {
                                    case ContentHAlign.FILL:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Center.X - ((float)guiObjects.Count / 2 * (cellSize + guiInterval) + guiInterval / 2) + (cellSize * (i % guiObjects.Count)) + (guiInterval * (i % guiObjects.Count + 1)));
                                        guiObjects.ElementAt(i).Value._rectangle.Width = cellSize;
                                        break;
                                    default:
                                    case ContentHAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Center.X - ((float)guiObjects.Count / 2 * (cellSize + guiInterval) + guiInterval / 2) + (cellSize * (i % guiObjects.Count)) + (guiInterval * (i % guiObjects.Count + 1)));
                                        guiObjects.ElementAt(i).Value._rectangle.Width = cellSize;
                                        break;
                                }
                                switch (contentVerticalAlign)
                                {
                                    case ContentVAlign.FILL:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = _rectangle.Top + guiInterval;
                                        guiObjects.ElementAt(i).Value._rectangle.Height = _rectangle.Height - guiInterval * 2;
                                        break;
                                    default:
                                    case ContentVAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (_rectangle.Center.Y - cellSize / 2);
                                        guiObjects.ElementAt(i).Value._rectangle.Height = cellSize;
                                        break;
                                }

                            } 
                        }
                        else // if HORIZONTAL and MULTIROW possible
                        {
                            int rowCount = 1;
                            int objInRow = 1;
                            int cellSize = 1;

                            while (true)
                            {
                                if (preferColCount != 0)
                                {
                                    objInRow = preferColCount > guiObjects.Count ? guiObjects.Count : preferColCount;
                                    rowCount = (int)Math.Ceiling((double)(guiObjects.Count) / objInRow);
                                }
                                else
                                    objInRow = (int)Math.Ceiling((double)(guiObjects.Count) / rowCount);

                                //get cellsize for one column
                                float vCof = (((float)(_rectangle.Height - ((rowCount +1) * guiInterval))) / ((float)(cellSize * rowCount)));
                                int tmpVCellSize = (int)Math.Floor((cellSize * (vCof + 0.001)));
                                //get cellsize for one row
                                float hCof = (((float)(_rectangle.Width - ((objInRow + 1) * guiInterval))) / ((float)(objInRow * cellSize)));
                                int tmpHCellSize = (int)Math.Floor((cellSize * (hCof + 0.001)));

                                cellSize = (tmpHCellSize > tmpVCellSize ? tmpVCellSize : tmpHCellSize);

                                if (cellSize >= Math.Floor((double)(_rectangle.Height - (guiInterval * (rowCount + 1)) - guiInterval) / (rowCount + 1)) || preferColCount != 0)
                                    break;
                                rowCount++;
                            }

                            rowCount = (int)Math.Ceiling((double)(guiObjects.Count) / objInRow);

                            for (int i = 0, row=0; i < guiObjects.Count; i++, row=(int)Math.Floor((double)i/objInRow))
                            {
                                switch(contentHorizontalAlign)
                                {
                                    case ContentHAlign.AUTOINTERVAL:
                                        int xInterval =(_rectangle.Width - (objInRow * cellSize))/ (objInRow + 1);
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Left + xInterval + (cellSize * (i % objInRow)) + (xInterval * (i % objInRow)));                                   
                                        break;
                                    default:
                                    case ContentHAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Center.X - ((float)objInRow / 2 * (cellSize + guiInterval) + guiInterval/2) + (cellSize * (i % objInRow)) + (guiInterval * (i % objInRow + 1)));
                                        break;
                                }
                                switch (contentVerticalAlign)
                                {
                                    case ContentVAlign.AUTOINTERVAL:
                                        int yInterval = (_rectangle.Height - (rowCount * cellSize)) / (rowCount + 1);
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Top + yInterval + (row * cellSize) + ((row) * yInterval));
                                        break;
                                    case ContentVAlign.TOP:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = _rectangle.Top + (row * (cellSize + guiInterval)) + guiInterval;
                                        break;
                                    case ContentVAlign.BOTTOM:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Bottom- ((float)rowCount * (cellSize + guiInterval)) + (row * (cellSize + guiInterval)));
                                        break;
                                    default:
                                    case ContentVAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - ((float)rowCount / 2 * (cellSize + guiInterval)-guiInterval/2) + (row * (cellSize + guiInterval)));
                                        break;

                                }

                                guiObjects.ElementAt(i).Value._rectangle.Width = cellSize;
                                guiObjects.ElementAt(i).Value._rectangle.Height = cellSize;
                            }
                        }
                    }
                    else if (guiDirection == GUIDirection.VERTICAL)
                    {
                        if (contentMultiLine == ContentMultiLine.IMPOSSIBLE)
                        {
                            int cellSize = (_rectangle.Width > _rectangle.Height ? _rectangle.Height : _rectangle.Width) - guiInterval * 2;
                            if (cellSize < 1)
                                cellSize = 0;

                            //get cellsize for one column
                            float vCof = (((float)(_rectangle.Height - ((guiObjects.Count + 1) * guiInterval))) / ((float)(cellSize * guiObjects.Count)));
                            if(vCof<1)
                                cellSize = (int)Math.Floor((cellSize * (vCof + 0.001)));

                            for (int i = 0; i < guiObjects.Count; i++)
                            {
                                switch (contentHorizontalAlign)
                                {
                                    case ContentHAlign.FILL:
                                        guiObjects.ElementAt(i).Value._rectangle.X = _rectangle.Left + guiInterval;
                                        guiObjects.ElementAt(i).Value._rectangle.Width = _rectangle.Width - guiInterval*2;
                                        break;
                                    default:
                                    case ContentHAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (_rectangle.Center.X - cellSize / 2);
                                        guiObjects.ElementAt(i).Value._rectangle.Width = cellSize;
                                        break;
                                }
                                switch (contentVerticalAlign)
                                {
                                    case ContentVAlign.FILL:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - ((float)guiObjects.Count / 2 * (cellSize + guiInterval) + guiInterval / 2) + (cellSize * (i % guiObjects.Count)) + (guiInterval * (i % guiObjects.Count + 1)));
                                        guiObjects.ElementAt(i).Value._rectangle.Height = cellSize;
                                        break;
                                    default:
                                    case ContentVAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - ((float)guiObjects.Count / 2 * (cellSize + guiInterval) + guiInterval / 2) + (cellSize * (i % guiObjects.Count)) + (guiInterval * (i % guiObjects.Count + 1)));
                                        guiObjects.ElementAt(i).Value._rectangle.Height = cellSize;
                                        break;
                                }
                            }
                        }
                        else // if VERTICAL and MULTICOL possible
                        {
                            int colCount = 1;
                            int objInCol = 1;
                            int cellSize = 1;

                            while (true)
                            {
                                if (preferRowCount != 0)
                                {
                                    objInCol = preferRowCount > guiObjects.Count ? guiObjects.Count : preferRowCount;
                                    colCount = (int)Math.Ceiling((double)(guiObjects.Count) / objInCol);
                                }
                                else
                                    objInCol = (int)Math.Ceiling((double)(guiObjects.Count) / colCount);

                                //objInCol = (int)Math.Ceiling((double)(guiObjects.Count) / colCount);

                                //get cellsize for one column
                                float vCof = (((float)(_rectangle.Height - ((objInCol+1) * guiInterval))) / ((float)(cellSize * objInCol)));
                                int tmpVCellSize = (int)Math.Floor((cellSize * (vCof + 0.001)));
                                //get cellsize for one row
                                float hCof = (((float)(_rectangle.Width - ((colCount+1) * guiInterval))) / ((float)(colCount * cellSize)));
                                int tmpHCellSize = (int)Math.Floor((cellSize * (hCof + 0.001)));

                                cellSize = (tmpHCellSize > tmpVCellSize ? tmpVCellSize : tmpHCellSize);

                                if (cellSize >= Math.Floor((double)(_rectangle.Width - (guiInterval * (colCount + 1)) - guiInterval) / (colCount + 1)) || preferRowCount!=0)
                                    break;
                                colCount++;
                            }

                            colCount = (int)Math.Ceiling((double)(guiObjects.Count) / objInCol);

                            for (int i = 0, col = 0; i < guiObjects.Count; i++, col = (int)Math.Floor((double)i / objInCol))
                            {
                                switch (contentHorizontalAlign)
                                {
                                    case ContentHAlign.AUTOINTERVAL:
                                        int xInterval = (_rectangle.Width - (colCount * cellSize)) / (colCount + 1);
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Left + xInterval + (col * cellSize) + ((col) * xInterval));
                                        break;
                                    case ContentHAlign.LEFT:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Left + (col * cellSize) + ((col + 1) * guiInterval));
                                        break;
                                    case ContentHAlign.RIGHT:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Right - ((float)colCount / 2 * (cellSize + guiInterval) + guiInterval / 2) + (col * cellSize) + ((col + 1) * guiInterval));
                                        break;

                                    default:
                                    case ContentHAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Center.X - ((float)colCount / 2 * (cellSize + guiInterval) + guiInterval / 2) + (col * cellSize) + ((col + 1) * guiInterval)); 
                                        break;
                                }
                                switch (contentVerticalAlign)
                                {
                                    case ContentVAlign.AUTOINTERVAL:
                                        int yInterval = (_rectangle.Height - (objInCol * cellSize)) / (objInCol + 1);
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Top + yInterval + (cellSize * (i % objInCol)) + (yInterval * (i % objInCol)));
                                        break;
                                    case ContentVAlign.TOP:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = _rectangle.Top + ((i % objInCol) * (cellSize + guiInterval)) + guiInterval;
                                        break;
                                    case ContentVAlign.BOTTOM:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Bottom - ((float)objInCol * (cellSize + guiInterval)) + ((i % objInCol) * (cellSize + guiInterval)));
                                        break;
                                    default:
                                    case ContentVAlign.CENTER:
                                        guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - ((float)objInCol / 2 * (cellSize + guiInterval) + guiInterval / 2) + (cellSize * (i % objInCol)) + (guiInterval * (i % objInCol+1)));
                                        break;
                                }
                                guiObjects.ElementAt(i).Value._rectangle.Width = cellSize;
                                guiObjects.ElementAt(i).Value._rectangle.Height = cellSize;
                            }
                        }
                    }
                    break;
                case GUITypePosition.AUTO_POS_USER_SIZE:

                    int rowWidth = 0;
                    int rowHeight = 0;
                    for (int i = 0; i < guiObjects.Count; i++)
                    {
                        rowWidth += guiObjects.ElementAt(i).Value._rectangle.Width;
                        rowHeight = rowHeight < guiObjects.ElementAt(i).Value._rectangle.Height ? guiObjects.ElementAt(i).Value._rectangle.Height : rowHeight;
                    }

                    if (guiDirection == GUIDirection.HORIZONTAL && newItem!=null)
                    {
                        switch(contentMultiLine)
                        {
                            case ContentMultiLine.IMPOSSIBLE:

                                    switch (contentHorizontalAlign)
                                    {
                                        case ContentHAlign.AUTOINTERVAL:

                                            int xInterval = (_rectangle.Width - rowWidth) / (guiObjects.Count + 1);
                                            for (int i = 0; i < guiObjects.Count; i++)
                                            {
                                                //int yInterval = (_rectangle.Height - (rowCount * cellSize)) / (rowCount + 1);
                                                guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Right - rowWidth - (xInterval * (guiObjects.Count - (i + 1)) + xInterval));
                                                guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - rowHeight / 2);
                                                rowWidth -= guiObjects.ElementAt(i).Value._rectangle.Width;
                                            }
                                            break;
                                        case ContentHAlign.CENTER:
                                            int currRowWidth = 0;
                                            for (int i = 0; i < guiObjects.Count; i++)
                                            {
                                                guiObjects.ElementAt(i).Value._rectangle.X = (int)_rectangle.Left + ((_rectangle.Width - (rowWidth + ((guiObjects.Count - 1) * guiInterval))) / 2) + currRowWidth+ i*guiInterval ;
                                                guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - rowHeight / 2);
                                                //rowWidth -= guiObjects.ElementAt(i).Value._rectangle.Width;
                                                currRowWidth+= guiObjects.ElementAt(i).Value._rectangle.Width;
                                        }
                                            break;
                                    }
                                /*switch (contentVerticalAlign)
                                {
                                    case ContentAlign.AUTOINTERVAL:

                                        int xInterval = (_rectangle.Width - rowWidth) / (guiObjects.Count + 1);
                                        for (int i = 0; i < guiObjects.Count; i++)
                                        {
                                            //int yInterval = (_rectangle.Height - (rowCount * cellSize)) / (rowCount + 1);
                                            guiObjects.ElementAt(i).Value._rectangle.X = (int)(_rectangle.Right - rowWidth - (xInterval * (guiObjects.Count - (i + 1)) + xInterval));
                                            guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - rowHeight / 2);
                                            rowWidth -= guiObjects.ElementAt(i).Value._rectangle.Width;
                                        }
                                        break;
                                    case ContentAlign.CENTER:
                                        int currRowWidth = 0;
                                        for (int i = 0; i < guiObjects.Count; i++)
                                        {
                                            guiObjects.ElementAt(i).Value._rectangle.X = (int)_rectangle.Left + ((_rectangle.Width - (rowWidth + ((guiObjects.Count - 1) * guiInterval))) / 2) + currRowWidth + i * guiInterval;
                                            guiObjects.ElementAt(i).Value._rectangle.Y = (int)(_rectangle.Center.Y - rowHeight / 2);
                                            //rowWidth -= guiObjects.ElementAt(i).Value._rectangle.Width;
                                            currRowWidth += guiObjects.ElementAt(i).Value._rectangle.Width;
                                        }
                                        break;
                                }*/

                                break;
                            case ContentMultiLine.POSSIBLE:
                                throw new NotImplementedException();
                                var firstInRow = guiObjects.Last(item => item.Value._rectangle.Left == (_rectangle.Left + guiInterval)).Value;
                                newItem._rectangle.Location = new Point(firstInRow._rectangle.Left, firstInRow._rectangle.Bottom + guiInterval);
                                break;
                        }
                    }
                    else if( guiDirection==GUIDirection.VERTICAL && newItem != null)
                    {
                        throw new NotImplementedException();
                        switch (contentMultiLine)
                        {
                            case ContentMultiLine.IMPOSSIBLE:
                                //newItem._rectangle.Location = new Point(last._rectangle.Left, last._rectangle.Bottom + guiInterval);
                                break;
                            case ContentMultiLine.POSSIBLE:
                                var firstInLastCol = guiObjects.Last(item => item.Value._rectangle.Top == (_rectangle.Top + guiInterval)).Value;
                                newItem._rectangle.Location = new Point(firstInLastCol._rectangle.Right + guiInterval, firstInLastCol._rectangle.Top);
                                break;
                        }                       
                    }
                    
                    break;
                case GUITypePosition.USER_POS_USER_SIZE:
                    throw new NotImplementedException();
                    //Yeaaa boy...
                    break;
            }
        }

        private bool TryAddGuiObj(GuiObject guiObject)
        {
            switch (guiTypePosition)
            {
                case GUITypePosition.AUTO_POS_AUTO_SIZE:
                    return true;
                    break;
                case GUITypePosition.AUTO_POS_USER_SIZE:

                    if (guiDirection == GUIDirection.HORIZONTAL)
                    {
                        if(contentMultiLine ==ContentMultiLine.POSSIBLE)
                        {
                            throw new NotImplementedException();
                            /*Rectangle currRect = new Rectangle(_rectangle.Left, _rectangle.Top, guiObject._rectangle.Width, guiObject._rectangle.Height); 

                            for (int i = 0; i < guiObjects.Count+1; i++)
                            {
                                if(currRect.Right <= _rectangle.Right && currRect.Bottom <= _rectangle.Bottom)
                                {
                                    currRect.Offset(currRect.Width,)
                                }
                            }
                            switch (contentAlign)
                            {
                                case ContentAlign.AUTOINTERVAL:
                                    if (rowWidth <= _rectangle.Width)
                                        return true;
                                    break;
                                default:
                                    if ((rowWidth + (guiObjects.Count + 1) * guiInterval + guiInterval) <= _rectangle.Width)
                                        return true;
                                    break;
                            }*/
                        }
                        else if(contentMultiLine == ContentMultiLine.IMPOSSIBLE)
                        {
                            int rowWidth = guiObject._rectangle.Width;
                            int rowHeight = guiObject._rectangle.Height;
                            for (int i = 0; i < guiObjects.Count; i++)
                            {
                                rowWidth += guiObjects.ElementAt(i).Value._rectangle.Width;
                                rowHeight = rowHeight < guiObjects.ElementAt(i).Value._rectangle.Height ? guiObjects.ElementAt(i).Value._rectangle.Height : rowHeight;
                            }
                            switch (contentHorizontalAlign)
                            {
                                case ContentHAlign.AUTOINTERVAL:
                                    if (rowWidth <= _rectangle.Width)
                                        return true;
                                    break;
                                case ContentHAlign.CENTER:
                                    if ((rowWidth + (guiObjects.Count+1) * guiInterval + guiInterval) <= _rectangle.Width)
                                        return true;
                                    break;
                            }
                        }
                        return false;
                    }
                    else if (guiDirection == GUIDirection.VERTICAL)
                    {
                        if (contentMultiLine == ContentMultiLine.POSSIBLE)
                        {
                            throw new NotImplementedException();
                           /* var firstInLastCol = guiObjects.Last(item => item.Value._rectangle.Top == (_rectangle.Top + guiInterval)).Value;
                            if ((firstInLastCol._rectangle.Right + ((guiInterval < 0 ? 0 : guiInterval) * 2) + guiObject._rectangle.Width) < _rectangle.Right)
                            {
                                return true;
                            }*/
                        }
                        else if (contentMultiLine == ContentMultiLine.IMPOSSIBLE)
                        {
                            int rowWidth = guiObject._rectangle.Width;
                            int rowHeight = guiObject._rectangle.Height;
                            for (int i = 0; i < guiObjects.Count; i++)
                            {
                                rowWidth += guiObjects.ElementAt(i).Value._rectangle.Width;
                                rowHeight = rowHeight < guiObjects.ElementAt(i).Value._rectangle.Height ? guiObjects.ElementAt(i).Value._rectangle.Height : rowHeight;
                            }
                            switch (contentVerticalAlign)
                            {
                                case ContentVAlign.AUTOINTERVAL:
                                    if (rowHeight <= _rectangle.Height)
                                        return true;
                                    break;
                                case ContentVAlign.CENTER:
                                    if ((rowHeight + (guiObjects.Count+1) * guiInterval + guiInterval) < _rectangle.Height)
                                        return true;
                                    break;
                            }
                        }
                        return false;
                    }
                    break;
                case GUITypePosition.USER_POS_USER_SIZE:
                    if (!(guiObject._rectangle.Top >= _rectangle.Top
                        && guiObject._rectangle.Left >= _rectangle.Left
                        && guiObject._rectangle.Right <= _rectangle.Right
                        && guiObject._rectangle.Bottom <= _rectangle.Bottom))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    break;
            }
            return true;
        }


        private bool IsAddGUIPossible()
        {
            return false;
        }

        private void SetGUISize()
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            spriteBatch.Draw(DrawHelper.GetTexture(), destinationRectangle: _rectangle, color: backColor/*, layerDepth: layer*/);
            foreach (var guiObj in guiObjects.Values)
            {
                guiObj.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GUI.F());
            }
        }

        public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override bool Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            throw new NotImplementedException();
        }
    }
}
