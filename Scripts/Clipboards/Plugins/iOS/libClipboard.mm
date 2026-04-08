extern "C" void CopyToClipboard_Elysia(const char* text)
{
	[UIPasteboard generalPasteboard].string = [NSString stringWithUTF8String:text];
}
