import { useState } from 'react';

export default function Login() {
    const [showPassword, setShowPassword] = useState(false);

    return (
        <div className="min-h-screen flex items-center justify-center bg-base-200 px-4">
            <div className="w-full max-w-md">
                <div className="space-y-10">
                    <h2 className="text-3xl font-bold text-center text-base-content">
                        Login
                    </h2>

                    <form className="space-y-6">
                        <div className="form-control">
                            <label className="label">
                                <span className="label-text text-base-content/90">Email</span>
                            </label>
                            <input
                                type="email"
                                placeholder="Email"
                                className="input input-bordered input-lg w-full focus:input-primary bg-base-100/70 backdrop-blur-sm"
                            />
                        </div>

                        <div className="form-control space-y-2">
                            <label className="label">
                                <span className="label-text text-base-content/90">Password</span>
                            </label>
                            <div className="relative">
                                <input
                                    type={showPassword ? 'text' : 'password'}
                                    placeholder="Password"
                                    className="input input-bordered input-lg w-full focus:input-primary bg-base-100/70 backdrop-blur-sm pr-20"
                                />
                                <button
                                    type="button"
                                    className="absolute right-4 top-1/2 -translate-y-1/2 text-base-content/70 hover:text-base-content text-sm font-medium"
                                    onClick={() => setShowPassword(!showPassword)}
                                >
                                    {showPassword ? 'Hide' : 'Show'}
                                </button>
                            </div>

                            <div className="text-right">
                                <a href="#" className="link link-hover link-primary text-sm">
                                    Forgot Password?
                                </a>
                            </div>
                        </div>

                        <button className="btn btn-primary btn-lg w-full mt-6">
                            Login
                        </button>
                    </form>

                    <div className="divider text-base-content/40 my-8">OR</div>

                    <div className="space-y-3">
                        <button className="btn btn-primary btn-lg w-full">
                            Create Account
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}