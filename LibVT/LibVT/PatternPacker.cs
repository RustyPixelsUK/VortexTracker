using System;

namespace LibVT
{
    public struct PatternBlock
    {
        public int StartLine;
        public int EndLine;
        public int Speed;
        public int BaseSpeed;
        public int Step;
    }

    public class PatternsPacker
    {
        public int FromLine = 0;
        public int ToLine = 0;
        public Pattern Pattern;
        private Pattern ResPattern;
        private ChannelLine Chan0;
        private ChannelLine Chan1;
        private ChannelLine Chan2;
        //private TChildWin Child;
        private bool Validated = false;
        private bool EntirePattern = false;
        private int CurrentBlockNum = 0;
        private PatternBlock[] SrcBlocks;
        private PatternBlock[] DestBlocks;
        private PatternBlock SrcBlock;
        private PatternBlock DestBlock;
        private int RestLines = 0;

        public PatternsPacker()
        {
            //Child = activeWindow;
            Validated = false;
            Pattern = null;
            FromLine = -1;
            ToLine = -1;
        }

        public bool CantPack()
        {
            bool result = false;

            // Can pack
            if (Pattern == null)
                return result;

            if (FromLine == -1)
                return result;

            if (ToLine == -1)
                return result;

            Init();

            result = !EmptyLinesEnough();

            // success
            if (!result)
                Validated = true;

            return result;
        }

        public void Process()
        {
            if (!Validated)
                return;

            PrepareResultPattern();
            FindBlocks();
            PackBlocks();

            // Copy a new pattern to destination
            Pattern.Length = ResPattern.Length;
            Pattern.Lines = ResPattern.Lines;
            ToLine = DestBlocks[DestBlocks.Length].EndLine;

            Init();

            if (!EmptyLinesEnough())
                return;

            FindBlocks();
            PackBlocks();

            // Copy a new pattern to destination
            Pattern.Length = ResPattern.Length;
            Pattern.Lines = ResPattern.Lines;
        }

        private bool IsEmptyChan(ChannelLine chan)
        {
            return (chan.Note == -1) && (chan.Sample == 0) && (chan.Ornament == 0) && (chan.Volume == 0) && (chan.Envelope == 0) && (chan.AdditionalCommand.Number == 0) && (chan.AdditionalCommand.Delay == 0) && (chan.AdditionalCommand.Parameter == 0);
        }

        private bool IsEmptyLine(int lineIndex)
        {
            Chan0 = Pattern.Lines[lineIndex].Channel[0];
            Chan1 = Pattern.Lines[lineIndex].Channel[1];
            Chan2 = Pattern.Lines[lineIndex].Channel[2];

            return IsEmptyChan(Chan0) && IsEmptyChan(Chan1) && IsEmptyChan(Chan2) && (Pattern.Lines[lineIndex].Noise == 0) && (Pattern.Lines[lineIndex].Envelope == 0);
        }

        private bool LineHasSpeedCommand(int lineIndex)
        {
            return (ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Number == 0xB) || (ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Number == 0xB) || (ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Number == 0xB);
        }

        private bool EmptyLinesEnough()
        {
            int counter = 0;

            for (int i = FromLine; i <= ToLine; i++)
            {
                if (IsEmptyLine(i))
                    counter++;

                if (counter == 1)
                    return true;
            }

            return false;
        }

        private void PrepareResultPattern()
        {
            return;

            if (EntirePattern)
                return;

            for (int i = 0; i <= FromLine; i++)
                ResPattern.Lines[i] = Pattern.Lines[i];

            for (int i = ToLine; i <= Pattern.Length - 1; i++)
                ResPattern.Lines[i] = Pattern.Lines[i];
        }

        private void Init()
        {
            RestLines = 0;
            SrcBlocks = new PatternBlock[0];
            DestBlocks = new PatternBlock[0];

            for (int i = 0; i < VTModule.MaxPatternLength; i++)
            {
                ResPattern.Lines[i].Channel[0] = new ChannelLine();
                ResPattern.Lines[i].Channel[1] = new ChannelLine();
                ResPattern.Lines[i].Channel[2] = new ChannelLine();
            }

            EntirePattern = (FromLine == 0) && (ToLine == Pattern.Length - 1);

            // Set FromLine to non empty line
            if (IsEmptyLine(FromLine))
            {
                for (int i = FromLine; i <= Pattern.Length - 1; i++)
                {
                    if (!IsEmptyLine(i))
                    {
                        FromLine = i;
                        break;
                    }
                }
            }

            if (IsEmptyLine(ToLine))
            {
                for (int i = ToLine; i >= FromLine; i--)
                {
                    if (!IsEmptyLine(i))
                    {
                        RestLines = ToLine - i;
                        ToLine = i;
                        break;
                    }
                }
            }
        }

        private void FindBlocks()
        {
            int emptyLineCounter = 0;
            int lineIndex = FromLine;
            bool lineIsEmpty;
            int blockIndex = 0;
            PatternBlock srcBlock;
            PatternBlock destBlock;

            while (lineIndex <= ToLine)
            {
                lineIsEmpty = IsEmptyLine(lineIndex);

                // Line is empty, then increase empty lines counter
                if (lineIsEmpty)
                {
                    lineIndex++;
                    emptyLineCounter++;
                    continue;
                }

                // Line is not empty...
                // Start a new block
                if (emptyLineCounter == 0)
                {
                    // Finalize previous block, if exists
                    if (lineIndex > FromLine)
                    {
                        SrcBlocks[blockIndex].EndLine = lineIndex - 1;
                        DestBlocks[blockIndex].EndLine = lineIndex - 1;
                    }

                    // Create new a block
                    blockIndex = SrcBlocks.Length;
                    SrcBlocks = new PatternBlock[blockIndex + 1];
                    SrcBlocks[blockIndex].StartLine = lineIndex;
                    SrcBlocks[blockIndex].EndLine = -1;
                    DestBlocks = new PatternBlock[blockIndex + 1];
                    DestBlocks[blockIndex].StartLine = lineIndex;
                    DestBlocks[blockIndex].EndLine = -1;
                    lineIndex++;
                    continue;
                }

                // Reset empty lines counter
                if (emptyLineCounter > 0)
                    emptyLineCounter = 0;

                lineIndex++;
            }

            // Finalize last block
            if (SrcBlocks[blockIndex].EndLine == -1)
            {
                SrcBlocks[blockIndex].EndLine = lineIndex - 1;
                DestBlocks[blockIndex].EndLine = lineIndex - 1;
            }

            // Check for skipped lines
            blockIndex = 0;
            do
            {
                srcBlock = SrcBlocks[blockIndex];
                destBlock = DestBlocks[blockIndex];

                CalculateBlockSpeed(blockIndex);

                lineIndex = srcBlock.StartLine;

                do
                {
                    if (lineIndex == srcBlock.StartLine)
                    {
                        lineIndex += srcBlock.Step;
                        continue;
                    }

                    // Line skipped - insert new block
                    if (!IsEmptyLine(lineIndex - 1))
                    {
                        // Create a new block
                        SrcBlocks = new PatternBlock[SrcBlocks.Length + 1];
                        DestBlocks = new PatternBlock[DestBlocks.Length + 1];

                        // Shift blocks
                        for (int i = SrcBlocks.Length; i >= blockIndex + 1; i--)
                        {
                            SrcBlocks[i] = SrcBlocks[i - 1];
                            DestBlocks[i] = DestBlocks[i - 1];
                        }

                        // Fix next block params
                        SrcBlocks[blockIndex + 1].StartLine = lineIndex - 1;
                        DestBlocks[blockIndex + 1].StartLine = lineIndex - 1;

                        // Set a new block params
                        SrcBlocks[blockIndex].EndLine = lineIndex - 2;
                        DestBlocks[blockIndex].EndLine = lineIndex - 2;
                        break;
                    }

                    lineIndex += srcBlock.Step;
                }
                while (lineIndex < srcBlock.EndLine);

                blockIndex++;

                if (blockIndex > SrcBlocks.Length)
                    break;

            }
            while (true);

            // If empty lines in pattern bottom, then add new block
            if (RestLines > 0)
            {
                SrcBlocks = new PatternBlock[SrcBlocks.Length + 1];
                DestBlocks = new PatternBlock[DestBlocks.Length + 1];
                SrcBlocks[blockIndex].StartLine = SrcBlocks[blockIndex - 1].EndLine + 1;
                SrcBlocks[blockIndex].EndLine = SrcBlocks[blockIndex].StartLine + RestLines - 1;
                DestBlocks[blockIndex].StartLine = SrcBlocks[blockIndex].StartLine;
                DestBlocks[blockIndex].EndLine = SrcBlocks[blockIndex].EndLine;

                CalculateBlockSpeed(blockIndex);
            }
        }

        private int GetBlockSpeed(int blockIndex)
        {
            int result;
            int distance;
            int notEmptyLineNum;
            PatternBlock srcBlock = SrcBlocks[blockIndex];

            if (srcBlock.EndLine - srcBlock.StartLine <= 1)
            {
                srcBlock.Step = srcBlock.EndLine - srcBlock.StartLine;
                result = srcBlock.BaseSpeed;
                return result;
            }

            notEmptyLineNum = -1;
            srcBlock.Step = srcBlock.EndLine - srcBlock.StartLine;

            // Find minimal distance between lines
            for (int i = srcBlock.StartLine; i <= srcBlock.EndLine; i++)
            {
                if (IsEmptyLine(i))
                    continue;

                if (notEmptyLineNum == -1)
                {
                    notEmptyLineNum = i;
                    continue;
                }

                distance = i - notEmptyLineNum;

                if (distance < srcBlock.Step)
                    srcBlock.Step = distance;

                notEmptyLineNum = i;
            }

            result = srcBlock.Step * srcBlock.BaseSpeed;

            return result;
        }

        private int GetBaseSpeed(int blockIndex)
        {
            int result;
            // Init
            //TChildWin.PlayingWindow[0] = Child;
            AY.ChipCount = 1;

            // Detect speed
            //Child.RerollToLineNum(0, SrcBlocks[blockIndex].StartLine, true);
            AppEvents.SendEvent(EventType.RerollToLineNum, 0, SrcBlocks[blockIndex].StartLine, true);
            result = VTModule.PlayArgs[VTModule.ChipIndex].Delay;

            return result;
        }

        private void CalculateBlockSpeed(int blockIndex)
        {
            PatternBlock srcBlock = SrcBlocks[blockIndex];
            PatternBlock destBlock = DestBlocks[blockIndex];
            srcBlock.BaseSpeed = GetBaseSpeed(blockIndex);
            srcBlock.Speed = GetBlockSpeed(blockIndex);
            destBlock.Speed = srcBlock.Speed;
            destBlock.BaseSpeed = srcBlock.BaseSpeed;
            destBlock.Step = srcBlock.Step;
        }

        private void UpdateSpeedCommands(int lineIndex)
        {
            Chan0 = ResPattern.Lines[lineIndex].Channel[0];
            Chan1 = ResPattern.Lines[lineIndex].Channel[1];
            Chan2 = ResPattern.Lines[lineIndex].Channel[2];

            if (lineIndex == DestBlock.EndLine && (Chan0.AdditionalCommand.Number == 0xB || Chan1.AdditionalCommand.Number == 0xB || Chan2.AdditionalCommand.Number == 0xB))
                return;

            if (Chan0.AdditionalCommand.Number == 0xB)
                Chan0.AdditionalCommand.Parameter += (byte)DestBlock.Speed;
            else if (Chan1.AdditionalCommand.Number == 0xB)
                Chan1.AdditionalCommand.Parameter += (byte)DestBlock.Speed;
            else if (Chan2.AdditionalCommand.Number == 0xB)
                Chan2.AdditionalCommand.Parameter += (byte)DestBlock.Speed;
        }

        private void PackBlock()
        {
            int blockLength;
            int lineIndex;
            int srcLine;

            if (CurrentBlockNum > 0)
                DestBlock.StartLine = DestBlocks[CurrentBlockNum - 1].EndLine + 1;

            srcLine = SrcBlock.StartLine;
            blockLength = SrcBlock.EndLine - SrcBlock.StartLine;

            DestBlock.EndLine = blockLength == 0 ? DestBlock.StartLine : DestBlock.StartLine + (blockLength / SrcBlock.Step);

            // Just copy line without compression
            if (DestBlock.StartLine == DestBlock.EndLine)
            {
                lineIndex = DestBlock.StartLine;
                ResPattern.Lines[lineIndex] = Pattern.Lines[srcLine];
                return;
            }

            // Compress
            for (int i = DestBlock.StartLine; i <= DestBlock.EndLine; i++)
            {
                ResPattern.Lines[i] = Pattern.Lines[srcLine];

                UpdateSpeedCommands(i);

                if (i < DestBlock.EndLine)
                    srcLine += SrcBlock.Step;
            }

            // Set block start speed
            SetSpeedCommand(DestBlock.StartLine, DestBlock.Speed, true);

            // Restore base speed after
            SetSpeedCommand(DestBlock.EndLine, DestBlock.BaseSpeed, false);
        }

        private void SetSpeedCommand(int lineIndex, int speed, bool startSpeed)
        {
            if (!startSpeed && speed == DestBlock.Speed)
                return;

            if (lineIndex == DestBlock.EndLine && LineHasSpeedCommand(lineIndex))
                return;

            Chan0 = ResPattern.Lines[lineIndex].Channel[0];
            Chan1 = ResPattern.Lines[lineIndex].Channel[1];
            Chan2 = ResPattern.Lines[lineIndex].Channel[2];

            if (startSpeed && LineHasSpeedCommand(lineIndex))
            {
                if (Chan0.AdditionalCommand.Number == 0xB)
                    Chan0.AdditionalCommand.Parameter = (byte)speed;
                else if (Chan1.AdditionalCommand.Number == 0xB)
                    Chan1.AdditionalCommand.Parameter = (byte)speed;
                else if (Chan2.AdditionalCommand.Number == 0xB)
                    Chan2.AdditionalCommand.Parameter = (byte)speed;

                return;
            }

            if (Chan0.AdditionalCommand.Number == 0)
            {
                Chan0.AdditionalCommand.Number = 0xB;
                Chan0.AdditionalCommand.Delay = 0;
                Chan0.AdditionalCommand.Parameter = (byte)speed;
            }
            else if (Chan1.AdditionalCommand.Number == 0)
            {
                Chan1.AdditionalCommand.Number = 0xB;
                Chan1.AdditionalCommand.Delay = 0;
                Chan1.AdditionalCommand.Parameter = (byte)speed;
            }
            else
            {
                Chan2.AdditionalCommand.Number = 0xB;
                Chan2.AdditionalCommand.Delay = 0;
                Chan2.AdditionalCommand.Parameter = (byte)speed;
            }
        }

        private int GetSpeedParam(int lineIndex)
        {
            int result = 0;

            if (ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Number == 0xB)
                result = ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Parameter;
            else if (ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Number == 0xB)
                result = ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Parameter;
            else if (ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Number == 0xB)
                result = ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Parameter;

            return result;
        }

        private void RemoveSpeedCommand(int lineIndex)
        {
            if (ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Number == 0xB)
            {
                ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Number = 0;
                ResPattern.Lines[lineIndex].Channel[0].AdditionalCommand.Parameter = 0;
            }
            else if (ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Number == 0xB)
            {
                ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Number = 0;
                ResPattern.Lines[lineIndex].Channel[1].AdditionalCommand.Parameter = 0;
            }
            else if (ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Number == 0xB)
            {
                ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Number = 0;
                ResPattern.Lines[lineIndex].Channel[2].AdditionalCommand.Parameter = 0;
            }
        }

        private void SpeedCommandsCleaner()
        {
            int speed;
            int lastSpeed = -1;

            for (int i = 0; i <= ResPattern.Lines.Length; i++)
            {
                if (!LineHasSpeedCommand(i))
                    continue;

                speed = GetSpeedParam(i);

                if (lastSpeed == -1)
                {
                    lastSpeed = speed;
                    continue;
                }

                if (speed == lastSpeed)
                    RemoveSpeedCommand(i);

                lastSpeed = speed;
            }
        }

        private void PackBlocks()
        {
            int srcLine;
            int blockIndex;

            for (blockIndex = 0; blockIndex <= SrcBlocks.Length; blockIndex++)
            {
                CurrentBlockNum = blockIndex;
                SrcBlock = SrcBlocks[blockIndex];
                DestBlock = DestBlocks[blockIndex];
                PackBlock();
            }

            if (EntirePattern)
                ResPattern.Length = DestBlock.EndLine + 1;
            else
            {
                for (int i = 0; i < DestBlocks[0].StartLine; i++)
                    ResPattern.Lines[i] = Pattern.Lines[i];

                int lineIndex = DestBlocks[SrcBlocks.Length].EndLine;

                for (srcLine = SrcBlocks[DestBlocks.Length].EndLine + 1; srcLine <= Pattern.Length - 1; srcLine++)
                {
                    lineIndex++;
                    ResPattern.Lines[lineIndex] = Pattern.Lines[srcLine];
                }

                ResPattern.Length = lineIndex + 1;
            }

            SpeedCommandsCleaner();
        }
    }
}

