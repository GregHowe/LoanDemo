"use client";
import { Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";

function DeniedContent() {
  const router = useRouter();
  const params = useSearchParams();
  const reason = params.get("reason");

  return (
    <div className="flex items-center justify-center min-h-screen bg-red-100">
      <div className="max-w-md w-full p-6 bg-red-600 text-white shadow rounded text-center">
        <h1 className="text-2xl font-bold mb-4">Application Denied ❌</h1>
        <p className="text-lg mb-6">
          {reason ? `Reason: ${reason}` : "Your loan application was denied."}
        </p>
        <button
          onClick={() => router.push("/")}
          className="bg-white text-red-700 px-4 py-2 rounded"
        >
          Back to Home
        </button>
      </div>
    </div>
  );
}

export default function DeniedPage() {
  return (
    <Suspense fallback={<div className="p-6">Loading...</div>}>
      <DeniedContent />
    </Suspense>
  );
}
