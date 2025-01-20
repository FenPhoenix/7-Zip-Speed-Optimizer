# 7-Zip Speed Optimizer

A tool to package 7z FMs in a way that allows FM loaders to scan them quickly, while retaining the high compression advantage of the 7z format.

Performance increase can be up to 11x for AngelLoader (v1.10.0 and later) and up to 70x for FMSel, depending on how conscientiously the FM was previously packaged.

Versions of AngelLoader prior to v1.10.0 gain a lesser advantage, up to 2.5x or so.

File size increase is generally less than 2%, with a small number up to 5% and one observed outlier at 8%. The majority are fractions of a percent larger at most, and a few are actually smaller.

<hr>

To show just how fast it is, I took a set of 1309 FMs and repacked them into .7z format in two different ways.

The first is the **Standard set**, packed with 7-Zip 24.09 64-bit using LZMA2, compression level 9 (Ultra), 12 threads, and the rest of the settings left at default.
The second is the **Optimized set**, packed with 7-Zip Speed Optimizer (which uses 7-Zip 24.09 64-bit), LZMA2, compression level 9 (Ultra), and 12 threads.

The FMs were subjected to a full-set single-threaded scan benchmark. The benchmark was run on a Ryzen 5600, with the FMs located on an Aorus Gen4 NVME 2TB SSD (GP-ASM2NE6200TTTD).

<img src="https://i.postimg.cc/sDj2hC8v/v1-9-9-single.png"/>

**2.5x faster**. Not bad. But let's run the same benchmark in AngelLoader v1.10.0:

<img src="https://i.postimg.cc/cL64FMdW/v1-10-0-single.png"/>

Over **11x faster**. Now we're talking. But how does it do in FMSel*?

<img src="https://i.postimg.cc/mZNDkgmv/FMSel-chart.png"/>

You're reading that right. Nearly **_70 times_** faster.

So indeed, this is not just for AngelLoader: *any* 7z-supporting loader - past, present, or future - will benefit from this tool.

But what about file size? Does this spectacular speed increase come at the cost of enormous archives?

**Absolutely not.** Most FMs' size difference is a fraction of a percent, and none are more than a single-digit percent larger:

<img src="https://i.postimg.cc/5Ns25kdp/file-size-chart.png"/>

---

*I have no easy way to benchmark FMSel properly, so tests had to be run and timed manually. But the difference is so vast that it hardly matters: however you slice it, the standard set takes multiple minutes and the optimized set takes a handful of seconds.

---

### How it works:

<img src="https://i.postimg.cc/Mp4ZNMGQ/7zLayout.png"/>

A "block" is a solid segment of compressed data in which one or more files may be located. If a block contains multiple files, those files are compressed together as one long run of data, with no separation between files. This improves compression.

Importantly, blocks represent the boundaries of random access: If a file is located at the start of a block, then it can be decompressed immediately, whereas if a file is located some ways into a block, then all data before it in that block must be decompressed and thrown away before the file itself can be decompressed.

Thus, the basic idea is to place files loaders will want to read at the start of blocks so that they can be accessed without needing to decompress any data in front of them. However, splitting a 7z file into too many blocks results in a poor compression ratio. Therefore, care has been taken to balance these two competing concerns.

Files which are very small and may not all need to be read can each be placed in their own block, maintaining instant access to any of them while having negligible effect on compression ratio.

To maintain good compression ratios, .mis files (which are among the largest files in an FM) are packed in the "remaining files" block. However, the smallest used .mis file is placed at the start of the block for fast access. For Thief 1 and 2, a "used" .mis file means one that is specified in missflag.str and not marked "skip". For System Shock 2, all .mis files are considered "used". For Thief 3, no mission files need to be read to detect the game type, so this logic doesn't apply.

The reason for distinguishing "used" .mis files is that unused .mis files are sometimes dummy files that were created to placate DarkLoader or for other reasons. AngelLoader's game detector requires a validly structured .mis file in order to work, and it ignores unused .mis files for this reason. Some loaders may use alternative means of detecting valid .mis files such as "DEADBEEF" byte sequence detection, and will read the first valid .mis file they come across, rather than the smallest. However, the optimized layout works for them too: the fact that the smallest used .mis file is placed first in its block also means it's placed before any other .mis file in the entries list, so any loader taking the first valid .mis file it sees will automatically get the file that is both smallest and also at the start of its block.

The "main_x" images are placed in their own block because GarrettLoader used them as thumbnails when viewing FMs, and other loaders in the future may also want to do so. Furthermore, it was found that placing these images in their own block had negligible effect on 7z file size.
