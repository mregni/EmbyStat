import {VideoStream} from '../models/common';

export const generateStreamChipLabel = (stream: VideoStream) => {
  const bitDepth = stream?.BitDepth !== undefined ? `${stream?.BitDepth}bit - ` : '';
  return `${stream.height}x${stream.width} - 
  ${+((stream.bitRate ?? 0) / 1048576).toFixed(2)}Mbps - 
  ${bitDepth} ${stream.codec} - 
  ${stream.videoRange}`;
};
