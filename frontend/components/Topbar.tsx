"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useSession } from "@/lib/api";

export default function Topbar() {
  const pathname = usePathname();
  const router = useRouter();
  const { userName, isAdmin, logout } = useSession();

  return (
    <header className="topbar">
      <div className="wrap">
        <Link href="/" className="logo">IMDB</Link>
        <nav>
          <Link href="/" className={pathname === "/" ? "active" : ""}>Browse</Link>
          {isAdmin && <Link href="/admin" className={pathname.startsWith("/admin") ? "active" : ""}>Admin</Link>}
        </nav>
        <div className="grow" />
        {userName ? (
          <div className="userchip">
            {isAdmin && <span className="tag-admin">ADMIN</span>}
            {userName}
            <button className="btn ghost small" onClick={() => { logout(); router.push("/"); }}>Log out</button>
          </div>
        ) : (
          <div className="userchip">
            <Link href="/login" className="btn ghost small">Sign in</Link>
            <Link href="/register" className="btn small">Register</Link>
          </div>
        )}
      </div>
    </header>
  );
}
