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
