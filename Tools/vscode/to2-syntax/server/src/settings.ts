export interface To2LspSettings {
  libraryPath: string;
  localLibraryPath: string;
  maxNumberOfProblems: number;
}

export const defaultSettings: To2LspSettings = {
  libraryPath: "",
  localLibraryPath: "",
  maxNumberOfProblems: 1000,
};
