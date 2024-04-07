export interface To2LspSettings {
  libraryPath: string;
  localLibraryPath: string;
  maxNumberOfProblems: number;
  referenceJson: string;
}

export const defaultSettings: To2LspSettings = {
  libraryPath: "",
  localLibraryPath: "",
  maxNumberOfProblems: 1000,
  referenceJson: "",
};
