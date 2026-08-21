"use client";
import { useRouter } from "next/navigation";

export default function ApprovedPage() {
  const router = useRouter();

  return (
    <main className="result-shell result-approved">
      <div className="result-panel">
        <div className="result-icon" aria-hidden="true">✓</div>
        <p className="eyebrow">NIURO / APPLICATION STATUS</p>
        <h1>Application approved</h1>
        <p>Your loan application has been successfully approved.</p>
        <button
          onClick={() => router.push("/")}
          className="result-button"
        >
          Start a new application <span aria-hidden="true">→</span>
        </button>
      </div>
    </main>
  );
}
