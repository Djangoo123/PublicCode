'use client';

import React, { useEffect } from "react";
import { usePathname } from "next/navigation";

export default function ScrollToTop() {
    const path = usePathname();

    // Force to scroll to top of the window
    useEffect(() => {
        window.scrollTo(0, 0);
    }, [path]
    );

    return null;
}