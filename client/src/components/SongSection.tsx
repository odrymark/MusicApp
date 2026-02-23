import { forwardRef } from "react";

export interface Song {
    id: number;
    title: string;
    cover: string;
}

interface SongSectionProps {
    title: string;
    songs: Song[];
}

const SongSection = forwardRef<HTMLDivElement, SongSectionProps>(
    ({ title, songs }, ref) => {
        const scrollRow = (direction: "left" | "right") => {
            if (ref && "current" in ref && ref.current) {
                ref.current.scrollBy({
                    left: direction === "left" ? -300 : 300,
                    behavior: "smooth",
                });
            }
        };

        return (
            <div className="mb-12">
                <h2 className="text-2xl font-semibold mb-4 text-primary">{title}</h2>

                <div className="relative">
                    <button
                        onClick={() => scrollRow("left")}
                        className="absolute left-0 top-1/2 -translate-y-1/2 z-10 bg-base-100/80 hover:bg-base-100 p-2 rounded-full shadow"
                    >
                        ◀
                    </button>

                    <div
                        ref={ref}
                        className="flex gap-4 overflow-hidden scroll-smooth px-12"
                    >
                        {songs.map((song) => (
                            <div
                                key={song.id}
                                className="flex-shrink-0 w-40 cursor-pointer hover:scale-105 transition-transform"
                            >
                                <img
                                    src={song.cover}
                                    alt={song.title}
                                    className="rounded-lg w-full h-40 object-cover mb-2"
                                />
                                <p className="text-center text-sm font-medium">
                                    {song.title}
                                </p>
                            </div>
                        ))}
                    </div>

                    <button
                        onClick={() => scrollRow("right")}
                        className="absolute right-0 top-1/2 -translate-y-1/2 z-10 bg-base-100/80 hover:bg-base-100 p-2 rounded-full shadow"
                    >
                        ▶
                    </button>
                </div>
            </div>
        );
    }
);

export default SongSection;
